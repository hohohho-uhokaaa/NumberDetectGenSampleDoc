using System;

namespace NumberDetectGenSampleDoc
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var config = new Configuration();
                using (var fontProvider = new FontProvider(config))
                using (var generator = new ImageGenerator(config, fontProvider))
                {
                    generator.Generate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"エラーが発生しました: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}