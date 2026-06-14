using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// テストPDFを生成するクラス
    /// </summary>
    public class ImageGenerator : IDisposable
    {
        // 設定オブジェクト（画像サイズ、フォント設定など）
        private readonly Configuration _config;

        // フォントプロバイダー（フォントの検出とロードを担当）
        private readonly FontProvider _fontProvider;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="config">設定オブジェクト</param>
        /// <param name="fontProvider">フォントプロバイダー</param>
        public ImageGenerator(Configuration config, FontProvider fontProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            
            // QuestPDFのライセンスを設定（コミュニティライセンス）
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// テストPDFを生成して保存する
        /// </summary>
        public void Generate()
        {
            Console.WriteLine("Fedora環境でSkiaSharpとQuestPDFを使用してテストPDFを生成しています...");

            // 設定ファイルからテキスト描画設定を読み込み
            Console.WriteLine($"設定ファイルを読み込み中: {_config.TextDrawingConfigFile}");
            var textConfigs = TextDrawingConfigLoader.Load(_config.TextDrawingConfigFile);
            Console.WriteLine($"設定ファイルから {textConfigs.Count} 件の描画設定を読み込みました。");

            // SkiaSharpでビットマップを作成して描画
            Console.WriteLine("SkiaSharpで描画画像を作成中...");
            byte[] imageBytes = CreateDrawingImage(textConfigs);

            // PDFドキュメントを作成
            Console.WriteLine($"PDFドキュメントを作成中: {_config.OutputFileName}");
            var document = new TextDrawingDocument(_config, textConfigs, imageBytes);
            document.GeneratePdf(_config.OutputFileName);

            // 完了メッセージを表示
            PrintCompletionMessage(_config.OutputFileName);
        }

        /// <summary>
        /// SkiaSharpを使用して描画画像を作成し、PNGバイト配列として返す
        /// </summary>
        private byte[] CreateDrawingImage(List<TextDrawingConfig> textConfigs)
        {
            using (var bitmap = new SKBitmap(_config.ImageWidth, _config.ImageHeight))
            using (var canvas = new SKCanvas(bitmap))
            {
                // 背景を白でクリア
                canvas.Clear(SKColors.White);

                // フォントプロバイダーからフォントタイプフェイスを取得
                using (var typeface = _fontProvider.GetFont())
                {
                    // 各設定を描画
                    foreach (var config in textConfigs)
                    {
                        // 描画オブジェクトを作成（個別の文字サイズを適用）
                        var font = new SKFont(typeface, config.FontSize);
                        var paint = new SKPaint
                        {
                            Color = _config.TextColor,
                            IsAntialias = true
                        };

                        // 回転を適用してテキストを描画
                        canvas.Save();
                        canvas.Translate(config.X, config.Y);
                        canvas.RotateDegrees(config.Rotation);
                        canvas.DrawText(config.Text, 0, 0, SKTextAlign.Left, font, paint);
                        canvas.Restore();
                    }
                }

                // 画像をPNGバイト配列にエンコード
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    return data.ToArray();
                }
            }
        }

        /// <summary>
        /// 完了メッセージを表示する
        /// </summary>
        /// <param name="outputPath">出力ファイルパス</param>
        private void PrintCompletionMessage(string outputPath)
        {
            Console.WriteLine("========================================");
            Console.WriteLine($"生成完了: {Path.GetFullPath(outputPath)}");
            Console.WriteLine("脆弱性のない最新のセキュアなPDFが生成されました。");
            Console.WriteLine("========================================");
        }

        /// <summary>
        /// リソースを解放する
        /// </summary>
        public void Dispose()
        {
            // フォントプロバイダーのリソースを解放
            _fontProvider?.Dispose();
        }
    }

    /// <summary>
    /// QuestPDF用のドキュメントクラス
    /// </summary>
    public class TextDrawingDocument : IDocument
    {
        private readonly Configuration _config;
        private readonly List<TextDrawingConfig> _textConfigs;
        private readonly byte[] _imageBytes;

        public TextDrawingDocument(Configuration config, List<TextDrawingConfig> textConfigs, byte[] imageBytes)
        {
            _config = config;
            _textConfigs = textConfigs;
            _imageBytes = imageBytes;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    // 1ページ目：テキスト描画ページ（画像を埋め込み）
                    page.Size(PageSizes.A4);
                    page.Margin(0);
                    
                    page.Content().Element(content =>
                    {
                        content
                            .Background(Colors.White)
                            .Image(_imageBytes);
                    });
                })
                .Page(page =>
                {
                    // 2ページ目：説明ページ
                    page.Size(PageSizes.A4);
                    page.Margin(50);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Calibri));
                    
                    page.Header().Element(header =>
                    {
                        header.Row(row =>
                        {
                            row.RelativeItem().Text("テキスト描画設定説明").Bold().FontSize(24);
                        });
                    });

                    page.Content().Element(content =>
                    {
                        content.Column(column =>
                        {
                            column.Spacing(20);
                            
                            // 説明文
                            column.Item().Text("このPDFは以下の設定ファイルに基づいて生成されました。");
                            column.Item().Text($"設定ファイル: {_config.TextDrawingConfigFile}");
                            
                            column.Item().Text("設定ファイルのフォーマット（CSV形式）:").Bold();
                            column.Item().Text("位置X,位置Y,文字サイズ,傾き（度）,テキスト");
                            
                            column.Item().Text("描画されたテキスト一覧:").Bold();
                            
                            // 各設定の詳細
                            foreach (var config in _textConfigs)
                            {
                                column.Item().Text($"テキスト: {config.Text}, 位置: ({config.X}, {config.Y}), " +
                                                 $"サイズ: {config.FontSize}, 傾き: {config.Rotation}°");
                            }
                        });
                    });
                    
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("ページ ");
                            x.CurrentPageNumber();
                        });
                });
        }
    }
}
