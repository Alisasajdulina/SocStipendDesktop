using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SocStipendDesktop.Services
{
    public static class CaptchaService
    {
        public static (string Code, BitmapImage Image) GenerateCaptcha(int width = 150, int height = 50)
        {
            // Генерация случайного кода
            string code = GenerateRandomCode(5);

            // Создаем визуал для рендеринга
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                // Белый фон
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                // Текст капчи
                var text = new FormattedText(
                    code,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    20,
                    Brushes.Black,
                    96);

                dc.DrawText(text, new Point(5, 5));

                // Добавляем шум
                var random = new Random();
                for (int i = 0; i < 50; i++)
                {
                    dc.DrawRectangle(
                        new SolidColorBrush(Color.FromRgb(
                            (byte)random.Next(256),
                            (byte)random.Next(256),
                            (byte)random.Next(256))),
                        null,
                        new Rect(
                            random.Next(width),
                            random.Next(height),
                            2, 2));
                }
            }

            // Рендерим в RenderTargetBitmap
            var renderTargetBitmap = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(visual);

            // Конвертируем в BitmapImage
            var bitmapImage = new BitmapImage();
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                encoder.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            return (code, bitmapImage);
        }

        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}