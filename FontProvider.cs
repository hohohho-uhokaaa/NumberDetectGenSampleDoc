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
        // 設定オブジェクト（フォントパスなどの情報を保持）
        private readonly Configuration _config;

        // キャッシュされたフォントタイプフェイス（再利用のため）
        private SKTypeface? _typeface;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="config">設定オブジェクト</param>
        public FontProvider(Configuration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// 利用可能なフォントを取得する
        /// </summary>
        /// <returns>フォントタイプフェイス</returns>
        public SKTypeface GetFont()
        {
            // すでにフォントが読み込まれている場合はキャッシュを返す
            if (_typeface != null)
            {
                return _typeface;
            }

            // 優先順位の高いフォントパスから順に検索
            foreach (var fontPath in _config.FontPaths)
            {
                // フォントファイルが存在するか確認
                if (File.Exists(fontPath))
                {
                    try
                    {
                        // フォントファイルからタイプフェイスを読み込み
                        _typeface = SKTypeface.FromFile(fontPath);
                        return _typeface;
                    }
                    catch (Exception ex)
                    {
                        // フォントの読み込みに失敗した場合は警告を表示して次のフォントを試す
                        Console.WriteLine($"フォントの読み込みに失敗しました: {fontPath}, エラー: {ex.Message}");
                    }
                }
            }

            // フォールバック: デフォルトフォント名を使用してシステムフォントを取得
            try
            {
                _typeface = SKTypeface.FromFamilyName(_config.DefaultFontName, SKFontStyle.Bold);
                return _typeface;
            }
            catch (Exception ex)
            {
                // すべてのフォント取得に失敗した場合は例外をスロー
                throw new InvalidOperationException(
                    $"フォントの初期化に失敗しました。デフォルトフォント '{_config.DefaultFontName}' も読み込めません。", ex);
            }
        }

        /// <summary>
        /// リソースを解放する
        /// </summary>
        public void Dispose()
        {
            // フォントタイプフェイスを解放
            _typeface?.Dispose();
            _typeface = null;
        }
    }
}
