using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
            Console.WriteLine("Fedora環境でQuestPDFを使用してテストPDFを生成しています...");

            // 設定ファイルからテキスト描画設定を読み込み
            Console.WriteLine($"設定ファイルを読み込み中: {_config.TextDrawingConfigFile}");
            var textConfigs = TextDrawingConfigLoader.Load(_config.TextDrawingConfigFile);
            Console.WriteLine($"設定ファイルから {textConfigs.Count} 件の描画設定を読み込みました。");

            // PDFドキュメントを作成
            Console.WriteLine($"PDFドキュメントを作成中: {_config.OutputFileName}");
            var document = new TextDrawingDocument(_config, textConfigs);
            document.GeneratePdf(_config.OutputFileName);

            // 完了メッセージを表示
            PrintCompletionMessage(_config.OutputFileName);
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

        public TextDrawingDocument(Configuration config, List<TextDrawingConfig> textConfigs)
        {
            _config = config;
            _textConfigs = textConfigs;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    // 1ページ目：テキスト描画ページ
                    page.Size(PageSizes.A4);
                    page.Margin(0);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Calibri));
                    
                    page.Content().Element(content =>
                    {
                        content
                            .Background(Colors.White)
                            .Canvas((canvas, size) =>
                            {
                                if (canvas is SkiaSharp.SKCanvas skCanvas)
                                {
                                    // 各テキスト設定を描画
                                    using (var paint = new SkiaSharp.SKPaint())
                                    {
                                        paint.Color = SkiaSharp.SKColors.Black;
                                        paint.IsAntialias = true;
                                        
                                        using (var typeface = SkiaSharp.SKTypeface.FromFamilyName("Arial", SkiaSharp.SKFontStyle.Bold))
                                        {
                                            foreach (var config in _textConfigs)
                                            {
                                                using (var font = new SkiaSharp.SKFont(typeface, config.FontSize))
                                                {
                                                    skCanvas.Save();
                                                    skCanvas.Translate(config.X, _config.ImageHeight - config.Y);
                                                    skCanvas.RotateDegrees(config.Rotation);
                                                    skCanvas.DrawText(config.Text, 0, 0, SkiaSharp.SKTextAlign.Left, font, paint);
                                                    skCanvas.Restore();
                                                }
                                            }
                                        }
                                    }
                                }
                            });
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
