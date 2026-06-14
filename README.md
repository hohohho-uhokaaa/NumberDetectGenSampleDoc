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

環境（bash / cshなど）を問わず、以下のコマンドで環境変数を一時的に適用してビルドを実行します。

```bash
env NUGET_PACKAGES="$HOME/.nuget/packages" dotnet build
```

> **💡 Devinを運用する際のヒント**  
> Devinは dotnet コマンドを試行する際、自身のシェル環境（csh等）に合わせて export や setenv を使い分けて自動復旧を試みます。エラーにめげることなく果敢にあらゆる手段を駆使して最後までがんばるDevinを、暖かく見守ってあげてください。

## 実行方法

環境変数を指定して、以下のコマンドで実行します。

```bash
env NUGET_PACKAGES="$HOME/.nuget/packages" dotnet run
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
- **ImageGenerator**: PDF生成の主要なロジック（SkiaSharpでの描画、PNG変換、QuestPDFでのPDF生成）
- **TextDrawingConfig**: 設定ファイルの読み込みとパース

### 実装アプローチ（Skia-First方式）

本プロジェクトはQuestPDFのCanvas API廃止による実行時例外を回避するため、以下の「Skia-First方式（事前レンダリング）」を採用しています：

1. **SkiaSharpでの描画**: 最初にSkiaSharpを使用してSKBitmap（2480 x 3508ピクセル）を生成し、CSVの座標・回転角に基づいて数字を描画
2. **PNG変換**: レンダリング完了後の画像をPNGバイト配列に変換
3. **QuestPDFでの埋め込み**: QuestPDFの`.Image(imageBytes)`を使用して画像をPDFに埋め込み

このアプローチにより、QuestPDFのバージョンアップに左右されない高い堅牢性と、実行時例外の完全な回避を両立しています。

### リファクタリングの利点

- **テスト容易性**: 各クラスが独立しているため、単体テストが容易
- **保守性**: 責任が分離されているため、変更の影響範囲が限定
- **再利用性**: 各クラスを他のプロジェクトで再利用可能
- **可読性**: コードの意図が明確で理解しやすい

## 開発の注意点（トラブルシューティング）

本プロジェクトの構築・リファクタリングにあたっては、自律型AIエージェント **Devin (SWE-1.6)** にベースコードを生成させ、そこで生じた難解なビルド・実行時エラーを **Gemini** が解析して原因と対策指示を生成、それを Devin への設計変更指示として段階的に投入しながらプロジェクトを完了させました。

以下は、その高度なAI連携デバッグの過程で直面した重要な技術的ハマりポイントと、最終的に導き出した解決策です。今後のメンテナンス時の注意事項として参照してください。

### 1. Linux環境における NuGet キャッシュの権限エラー

**【現象】**
Linux（Fedora等）環境でのビルド・復元時、NuGetがシステムルートに近い `/home/packages` などのディレクトリにキャッシュを書き込もうとし、`Access to the path '/home/packages' is denied / Permission denied` エラーで `dotnet restore` が物理的に失敗する場合があります。

**【対策】**
ビルドや実行の前には、必ず実行ユーザーのホームディレクトリ配下を指すように環境変数 `NUGET_PACKAGES` を明示的に指定、または `export`（csh環境では `setenv`）してください。

```bash
export NUGET_PACKAGES="$HOME/.nuget/packages"
```

### 2. QuestPDF (v2024.12.0) の Canvas API 廃止にともなう実行時例外

> **💡 共有知見**  
> この情報は、QuestPDF v2024.12.0 以降で同様の問題に直面している開発者のためにコミュニティへ共有されています。

**【現象】**
QuestPDF v2024.12.0 以降において、ドキュメント内で直接 SkiaSharp の生キャンバスを操作する `.Canvas()` APIを呼び出すと、ビルドは通るものの、実行時に強制的にクラッシュ（`Deprecated` 例外）を発生させる仕様に変更されました。また、代替とされる内部インターフェースもバージョンアップにより名称や名前空間が変更されるリスクがあります。

**【対策】**
QuestPDF 内部での高度なグラフィックス描画を完全に排除し、「Skia-First 方式（事前レンダリング）」を採用して画像を1枚絵として埋め込むことで、QuestPDFのバージョンに依存しない堅牢性を確保しました。

### 3. Linux環境における日本語・標準フォントの不足と文字化け防止

**【現象】**
Linux（Fedora等）環境では、Windows標準のフォントがデフォルトでインストールされていないため、SkiaSharpでの描画時に意図しないフォールバックフォントが適用されて文字位置がズレたり、QuestPDFで直接出力する2ページ目の日本語説明文が文字化け（トーフ化、???? 化）する可能性があります。

**【対策】**
Linux環境で実行する場合は、あらかじめシステムに適切な日本語フォントをインストールするか、プロジェクト内にフォントファイルを同梱して `Configuration.cs` の `FontPaths` から読み込ませてください。

Fedoraで標準的なオープンソースフォント（Google Noto Fonts等）を導入する場合のコマンド例：

```bash
sudo dnf install google-noto-sans-fonts google-noto-sans-cjk-fonts
```

### 4. 本質的な自動化へ向けた次のフェーズ（Devinへの指示例）

本プロジェクトで生成したテスト用PDFの精度検証が完了した後、実際の「複数ページに一括スキャンされたPDFから、所定位置の数字を読み取って自動リネーム・分割する本番用パイプライン」の構築をDevinへ指示する際のプロンプトの例です。

```
We are going to build a production tool to automate a miserable paperwork task.
The goal is:
1. Split a multi-page scanned PDF into individual 1-page PDFs.
2. Convert each page into a 300 DPI image (using PDFium or similar).
3. Crop a specific coordinates (ROI) where the target number is printed.
4. Recognize the number using Emgu.CV (OpenCV) or Tesseract.
5. Rename and save the 1-page PDF using the recognized number as the filename (e.g., 12345.pdf).

Let's create a prototype for this pipeline. First, recommend the best C# libraries for PDF splitting and rendering on Linux (Fedora).
```

## ライセンス

MIT License