using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NumberDetectGenSampleDoc
{
    /// <summary>
    /// テキスト描画設定を表すクラス
    /// </summary>
    public class TextDrawingConfig
    {
        /// <summary>
        /// X座標
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y座標
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 文字サイズ
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// 傾き（度）
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// 描画するテキスト
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// テキスト描画設定ファイルを読み込むクラス
    /// </summary>
    public class TextDrawingConfigLoader
    {
        /// <summary>
        /// 設定ファイルからテキスト描画設定を読み込む
        /// </summary>
        /// <param name="filePath">設定ファイルパス</param>
        /// <returns>テキスト描画設定のリスト</returns>
        public static List<TextDrawingConfig> Load(string filePath)
        {
            var configs = new List<TextDrawingConfig>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"設定ファイルが見つかりません: {filePath}");
            }

            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                // 空行やコメント行（#で始まる行）をスキップ
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                {
                    continue;
                }

                // カンマ区切りでパース
                var parts = line.Split(',');
                if (parts.Length != 5)
                {
                    Console.WriteLine($"警告: 無効な行をスキップします: {line}");
                    continue;
                }

                try
                {
                    var config = new TextDrawingConfig
                    {
                        X = float.Parse(parts[0].Trim()),
                        Y = float.Parse(parts[1].Trim()),
                        FontSize = float.Parse(parts[2].Trim()),
                        Rotation = float.Parse(parts[3].Trim()),
                        Text = parts[4].Trim()
                    };

                    configs.Add(config);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"警告: 行のパースに失敗しました: {line}, エラー: {ex.Message}");
                }
            }

            return configs;
        }
    }
}
