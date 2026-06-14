using System.Collections.Generic;
using SkiaSharp;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// 画像生成の設定値を管理するクラス
    /// </summary>
    public class Configuration
    {
        // ========================================
        // 画像サイズと出力設定
        // ========================================

        /// <summary>
        /// 画像の幅（A4サイズ @ 300 DPI）
        /// </summary>
        public int ImageWidth { get; set; } = 2480;

        /// <summary>
        /// 画像の高さ（A4サイズ @ 300 DPI）
        /// </summary>
        public int ImageHeight { get; set; } = 3508;

        /// <summary>
        /// 出力ファイル名（PDF形式）
        /// </summary>
        public string OutputFileName { get; set; } = "sample_300dpi.pdf";

        // ========================================
        // テキスト描画設定
        // ========================================

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public float FontSize { get; set; } = 140;

        /// <summary>
        /// テキストカラー（RGB）
        /// </summary>
        public SKColor TextColor { get; set; } = new SKColor(30, 35, 45);

        /// <summary>
        /// 回転角度の最小値（度）
        /// </summary>
        public float MinRotationDegree { get; set; } = -25;

        /// <summary>
        /// 回転角度の最大値（度）
        /// </summary>
        public float MaxRotationDegree { get; set; } = 25;

        // ========================================
        // テストデータ設定
        // ========================================

        /// <summary>
        /// テスト用の数字リスト（1桁、2桁、3桁を含む）
        /// </summary>
        public List<string> TestNumbers { get; set; } = new List<string>
        {
            "5", "0", "7",       // 1桁
            "42", "89", "10",    // 2桁
            "365", "702"         // 3桁
        };

        /// <summary>
        /// テキストの配置位置リスト（重なりを防ぐための座標）
        /// </summary>
        public List<SKPoint> TextPositions { get; set; } = new List<SKPoint>
        {
            new SKPoint(300, 500),
            new SKPoint(1400, 700),
            new SKPoint(500, 1300),
            new SKPoint(1500, 1700),
            new SKPoint(400, 2300),
            new SKPoint(1300, 2700),
            new SKPoint(800, 3100),
            new SKPoint(900, 900)
        };

        // ========================================
        // フォント設定
        // ========================================

        /// <summary>
        /// フォントパスの優先順位リスト（上から順に検索）
        /// </summary>
        public List<string> FontPaths { get; set; } = new List<string>
        {
            "/usr/share/fonts/liberation-sans/LiberationSans-Bold.ttf",
            "/usr/share/fonts/dejavu-sans-fonts/DejaVuSans-Bold.ttf"
        };

        /// <summary>
        /// デフォルトフォント名（フォールバック用：システムフォントが見つからない場合に使用）
        /// </summary>
        public string DefaultFontName { get; set; } = "sans-serif";

        // ========================================
        // 設定ファイル
        // ========================================

        /// <summary>
        /// テキスト描画設定ファイルのパス（CSV形式：位置X,位置Y,文字サイズ,傾き,テキスト）
        /// </summary>
        public string TextDrawingConfigFile { get; set; } = "text_drawing_config.csv";
    }
}
