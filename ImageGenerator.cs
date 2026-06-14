using System;
using System.IO;
using SkiaSharp;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// テスト画像を生成するクラス
    /// </summary>
    public class ImageGenerator : IDisposable
    {
        private readonly Configuration _config;
        private readonly FontProvider _fontProvider;
        private readonly Random _random;

        public ImageGenerator(Configuration config, FontProvider fontProvider)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _fontProvider = fontProvider ?? throw new ArgumentNullException(nameof(fontProvider));
            _random = new Random();
        }

        /// <summary>
        /// テスト画像を生成して保存する
        /// </summary>
        public void Generate()
        {
            Console.WriteLine("Fedora環境でSkiaSharpを使用してテスト画像を生成しています...");

            using (var bitmap = new SKBitmap(_config.ImageWidth, _config.ImageHeight))
            using (var canvas = new SKCanvas(bitmap))
            {
                DrawBackground(canvas);
                DrawTextElements(canvas);
                SaveImage(bitmap, _config.OutputFileName);

                PrintCompletionMessage(_config.OutputFileName);
            }
        }

        /// <summary>
        /// 背景を描画する
        /// </summary>
        private void DrawBackground(SKCanvas canvas)
        {
            canvas.Clear(SKColors.White);
        }

        /// <summary>
        /// テキスト要素を描画する
        /// </summary>
        private void DrawTextElements(SKCanvas canvas)
        {
            using (var typeface = _fontProvider.GetFont())
            {
                var (font, paint) = CreateDrawingObjects(typeface);
                int count = Math.Min(_config.TestNumbers.Count, _config.TextPositions.Count);

                for (int i = 0; i < count; i++)
                {
                    string text = _config.TestNumbers[i];
                    SKPoint position = _config.TextPositions[i];
                    float rotation = GenerateRandomRotation();

                    DrawRotatedText(canvas, text, font, paint, position, rotation);
                }
            }
        }

        /// <summary>
        /// ペイントオブジェクトを作成する
        /// </summary>
        private (SKFont font, SKPaint paint) CreateDrawingObjects(SKTypeface typeface)
        {
            var font = new SKFont(typeface, _config.FontSize);
            var paint = new SKPaint
            {
                Color = _config.TextColor,
                IsAntialias = true
            };
            return (font, paint);
        }

        /// <summary>
        /// ランダムな回転角度を生成する
        /// </summary>
        private float GenerateRandomRotation()
        {
            return (float)(_random.NextDouble() * (_config.MaxRotationDegree - _config.MinRotationDegree) + _config.MinRotationDegree);
        }

        /// <summary>
        /// 回転させたテキストを描画する
        /// </summary>
        private void DrawRotatedText(SKCanvas canvas, string text, SKFont font, SKPaint paint, SKPoint location, float degrees)
        {
            canvas.Save();
            canvas.Translate(location.X, location.Y);
            canvas.RotateDegrees(degrees);
            canvas.DrawText(text, 0, 0, SKTextAlign.Left, font, paint);
            canvas.Restore();
        }

        /// <summary>
        /// 画像を保存する
        /// </summary>
        private void SaveImage(SKBitmap bitmap, string outputPath)
        {
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(outputPath))
            {
                data.SaveTo(stream);
            }
        }

        /// <summary>
        /// 完了メッセージを表示する
        /// </summary>
        private void PrintCompletionMessage(string outputPath)
        {
            Console.WriteLine("========================================");
            Console.WriteLine($"生成完了: {Path.GetFullPath(outputPath)}");
            Console.WriteLine("脆弱性のない最新のセキュアな画像が生成されました。");
            Console.WriteLine("========================================");
        }

        public void Dispose()
        {
            _fontProvider?.Dispose();
        }
    }
}
