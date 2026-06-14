# NumberDetectGenSampleDoc

数字検出プログラムのテスト用画像を生成する .NET アプリケーションです。

## 概要

このプログラムは、SkiaSharp を使用して A4 サイズ（300 DPI、2480 x 3508 ピクセル）のテスト画像を生成します。生成される画像には、様々な角度で傾けられた数字（1桁、2桁、3桁）が含まれており、数字検出アルゴリズムのテストに使用できます。

## 特徴

- **クロスプラットフォーム対応**: SkiaSharp を使用しているため、Windows、Linux、macOS で動作します
- **A4 サイズ画像**: 300 DPI 基準の A4 サイズ（2480 x 3508 ピクセル）を生成
- **多様な数字**: 1桁、2桁、3桁の数字を含むテストデータ
- **ランダムな回転**: -25度〜+25度の範囲でランダムに回転した数字を描画
- **モジュラーアーキテクチャ**: 責任が明確に分離されたクラス構造

## プロジェクト構成

```
NumberDetectGenSampleDoc/
├── Program.cs              # エントリーポイント
├── Configuration.cs        # 設定値管理
├── FontProvider.cs         # フォント検出とロード
├── ImageGenerator.cs       # 画像生成ロジック
├── NumberDetectGenSampleDoc.csproj  # プロジェクトファイル
└── README.md               # このファイル
```

## 依存関係

- .NET 10.0
- SkiaSharp 3.119.4
- SkiaSharp.NativeAssets.Linux.NoDependencies 3.119.4

## ビルド方法

### 環境変数の設定（Linux の場合）

NuGet パッケージのキャッシュディレクトリを設定します：

```bash
setenv NUGET_PACKAGES $HOME/.nuget/packages
```

### ビルド

```bash
dotnet build
```

## 実行方法

```bash
# 環境変数を設定して実行
setenv NUGET_PACKAGES $HOME/.nuget/packages
dotnet run
```

実行すると、カレントディレクトリに `sample_300dpi.png` が生成されます。

## 設定

`Configuration.cs` で以下の設定を変更できます：

- `ImageWidth` / `ImageHeight`: 画像サイズ
- `FontSize`: フォントサイズ
- `TextColor`: テキストの色
- `MinRotationDegree` / `MaxRotationDegree`: 回転角度の範囲
- `TestNumbers`: 描画する数字のリスト
- `TextPositions`: テキストの配置位置
- `FontPaths`: フォントファイルの検索パス

## アーキテクチャ

### クラスの責任

- **Program**: アプリケーションのエントリーポイント、依存関係の組み立て、エラーハンドリング
- **Configuration**: すべての設定値を一元管理
- **FontProvider**: システムフォントの検出とロード、フォールバック処理
- **ImageGenerator**: 画像生成の主要なロジック（背景描画、テキスト描画、保存）

### リファクタリングの利点

- **テスト容易性**: 各クラスが独立しているため、単体テストが容易
- **保守性**: 責任が分離されているため、変更の影響範囲が限定
- **再利用性**: 各クラスを他のプロジェクトで再利用可能
- **可読性**: コードの意図が明確で理解しやすい

## ライセンス

このプロジェクトはサンプルコードとして提供されています。
