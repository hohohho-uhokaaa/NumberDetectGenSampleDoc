# NumberDetectGenSampleDoc

数字検出プログラムのテスト用PDFを生成する .NET アプリケーションです。

## 概要

このプログラムは、SkiaSharp と QuestPDF を使用して A4 サイズ（300 DPI、2480 x 3508 ピクセル）のテストPDFを生成します。生成されるPDFには、設定ファイルで指定した位置、サイズ、傾きで描画された数字が含まれており、数字検出アルゴリズムのテストに使用できます。

## 特徴

- **クロスプラットフォーム対応**: SkiaSharp を使用しているため、Windows、Linux、macOS で動作します
- **A4 サイズPDF**: 300 DPI 基準の A4 サイズ（2480 x 3508 ピクセル）を生成
- **設定ファイル対応**: CSV形式の設定ファイルで描画位置、文字サイズ、傾きを指定可能
- **2ページ構成**: 1ページ目に描画された数字、2ページ目に日本語の説明ページを含む
- **モジュラーアーキテクチャ**: 責任が明確に分離されたクラス構造

## プロジェクト構成

```
NumberDetectGenSampleDoc/
├── Program.cs              # エントリーポイント
├── Configuration.cs        # 設定値管理
├── FontProvider.cs         # フォント検出とロード
├── ImageGenerator.cs       # PDF生成ロジック
├── TextDrawingConfig.cs    # 設定ファイル読み込み
├── text_drawing_config.csv # テキスト描画設定ファイル（CSV）
├── NumberDetectGenSampleDoc.csproj  # プロジェクトファイル
└── README.md               # このファイル
```

## 依存関係

- .NET 10.0
- SkiaSharp 3.119.4
- SkiaSharp.NativeAssets.Linux.NoDependencies 3.119.4
- QuestPDF 2024.12.0

## ビルド方法

### 環境変数の設定（Linux の場合）

NuGet パッケージのキャッシュディレクトリを設定します：

```bash
export NUGET_PACKAGES="/home/tomjerry/.nuget/packages"
```

### ビルド

```bash
export NUGET_PACKAGES="/home/tomjerry/.nuget/packages"
dotnet build
```

## 実行方法

```bash
# 環境変数を設定して実行
export NUGET_PACKAGES="/home/tomjerry/.nuget/packages"
dotnet run
```

実行すると、カレントディレクトリに `sample_300dpi.pdf` が生成されます。

## 設定ファイル

`text_drawing_config.csv` ファイルで描画する数字の位置、サイズ、傾きを指定します。

### CSVフォーマット

```
位置X,位置Y,文字サイズ,傾き（度）,テキスト
```

- **位置X**: X座標（ピクセル）
- **位置Y**: Y座標（ピクセル）
- **文字サイズ**: フォントサイズ（ポイント）
- **傾き**: 回転角度（度）
- **テキスト**: 描画するテキスト

### サンプル設定ファイル

```
# テキスト描画設定ファイル
# フォーマット: 位置X,位置Y,文字サイズ,傾き（度）,テキスト
# コメント行は # で始めます

# 1桁の数字
400, 500, 140, 0, 5
1500, 1200, 140, 15, 0
800, 2800, 140, -10, 7

# 2桁の数字
500, 1600, 140, -15, 42
1600, 600, 140, 20, 89
1200, 2200, 140, -35, 10

# 3桁の数字
300, 700, 140, 5, 365
1400, 2700, 140, -5, 702
```

## 設定

`Configuration.cs` で以下の設定を変更できます：

- `ImageWidth` / `ImageHeight`: 画像サイズ
- `OutputFileName`: 出力PDFファイル名
- `TextDrawingConfigFile`: テキスト描画設定ファイルのパス
- `TextColor`: テキストの色
- `FontPaths`: フォントファイルの検索パス
- `DefaultFontName`: デフォルトフォント名（フォールバック用）

## アーキテクチャ

### クラスの責任

- **Program**: アプリケーションのエントリーポイント、依存関係の組み立て、エラーハンドリング
- **Configuration**: すべての設定値を一元管理
- **FontProvider**: システムフォントの検出とロード、フォールバック処理
- **ImageGenerator**: PDF生成の主要なロジック（背景描画、テキスト描画、説明ページ生成）
- **TextDrawingConfig**: 設定ファイルの読み込みとパース

### リファクタリングの利点

- **テスト容易性**: 各クラスが独立しているため、単体テストが容易
- **保守性**: 責任が分離されているため、変更の影響範囲が限定
- **再利用性**: 各クラスを他のプロジェクトで再利用可能
- **可読性**: コードの意図が明確で理解しやすい

## ライセンス

MIT License


## 開発の注意点（トラブルシューティング）

本プロジェクトの構築・リファクタリングにおいて、AIエージェントおよび開発環境が直面した重要な技術的ハマりポイントと解決策です。今後のメンテナンス時の注意事項として参照してください。

### 1. Linux環境における NuGet キャッシュの権限エラー

**【現象】**
Linux（Fedora等）環境でのビルド・復元時、NuGetがシステムルートに近い `/home/packages` などのディレクトリにキャッシュを書き込もうとし、`Access to the path '/home/packages' is denied / Permission denied` エラーで `dotnet restore` が物理的に失敗する場合があります。

**【対策】**
ビルドや実行の前には、必ず実行ユーザーのホームディレクトリ配下を指すように環境変数 `NUGET_PACKAGES` を明示的に指定、または `export` してください。

```bash
export NUGET_PACKAGES="$HOME/.nuget/packages"
```

### 2. QuestPDF (v2024.12.0) と SkiaSharp (v3.x) の型抽象化にともなうコンパイルエラー

**【現象】**
.NET 10.0 および SkiaSharp 3.119.4 の環境において、QuestPDFの `.Canvas()` または `DrawOnCanvas` APIを使用する際、ラムダ式の引数（キャンバスオブジェクト）を直接 `SkiaSharp.SKCanvas` 型として受け取ろうとすると、デリゲートの型不一致エラー（CS1678 / CS1661）が発生します。
逆に、型を指定しないと引数が `object` 型として推論され、`SKCanvas` 固有のメソッド（`Save()`, `Translate()`, `RotateDegrees()` 等）が呼べずに CS1061 エラーになります。

**【原因】**
QuestPDF v2024.12.0 以降、描画エンジンの抽象化のためにキャンバス引数のシグネチャが内部的に `object` 型に変更されたためです。

**【対策】（実装コードのルール）**
ラムダ式の引数は暗黙の型（または `object`）として受け取り、ブロック内部で型パターンマッチング（`is` 演算子）を使用して `SkiaSharp.SKCanvas` へキャストして使用してください。

```csharp
// ImageGenerator.cs での実装例
.Canvas((canvas, size) => 
{
    if (canvas is SkiaSharp.SKCanvas skCanvas)
    {
        skCanvas.Save();
        skCanvas.Translate(positionX, positionY);
        skCanvas.RotateDegrees(degrees);
        skCanvas.DrawText(text, 0, 0, paint);
        skCanvas.Restore();
    }
});
```
