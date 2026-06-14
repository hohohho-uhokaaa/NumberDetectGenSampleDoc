using System;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// アプリケーションのエントリーポイント
    /// </summary>
    class Program
    {
        /// <summary>
        /// アプリケーションのメインメソッド
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                // 設定オブジェクトを作成（デフォルト値を使用）
                var config = new Configuration();

                // フォントプロバイダーを作成（フォントの検出とロードを担当）
                using (var fontProvider = new FontProvider(config))
                // 画像ジェネレーターを作成（画像生成ロジックを担当）
                using (var generator = new ImageGenerator(config, fontProvider))
                {
                    // テスト画像を生成して保存
                    generator.Generate();
                }
            }
            catch (Exception ex)
            {
                // エラーが発生した場合はメッセージを表示して異常終了
                Console.WriteLine($"エラーが発生しました: {ex.Message}");
                Console.WriteLine($"スタックトレース: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
}