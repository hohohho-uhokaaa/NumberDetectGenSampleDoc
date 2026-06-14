using System;
using System.IO;
using SkiaSharp;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// フォントの検出と提供を行うクラス
    /// </summary>
    public class FontProvider : IDisposable
    {
        private readonly Configuration _config;
        private SKTypeface? _typeface;

        public FontProvider(Configuration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// 利用可能なフォントを取得する
        /// </summary>
        public SKTypeface GetFont()
        {
            if (_typeface != null)
            {
                return _typeface;
            }

            // 優先順位の高いフォントパスから検索
            foreach (var fontPath in _config.FontPaths)
            {
                if (File.Exists(fontPath))
                {
                    try
                    {
                        _typeface = SKTypeface.FromFile(fontPath);
                        return _typeface;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"フォントの読み込みに失敗しました: {fontPath}, エラー: {ex.Message}");
                    }
                }
            }

            // フォールバック: デフォルトフォント名を使用
            try
            {
                _typeface = SKTypeface.FromFamilyName(_config.DefaultFontName, SKFontStyle.Bold);
                return _typeface;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"フォントの初期化に失敗しました。デフォルトフォント '{_config.DefaultFontName}' も読み込めません。", ex);
            }
        }

        public void Dispose()
        {
            _typeface?.Dispose();
            _typeface = null;
        }
    }
}
