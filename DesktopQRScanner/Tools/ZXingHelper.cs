using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.Tools
{
    internal static class ZXingHelper
    {
        /// <summary>
        /// 扫描QR
        /// </summary>
        /// <param name="imageSource"></param>
        /// <returns></returns>
        public static string? ReadQRCode(BitmapSource bitmapSource)
        {
            ZXing.Windows.Compatibility.BarcodeReader reader = new();
            ZXing.Result result = reader.Decode(Convert2Bitmap(bitmapSource));
            return result.Text;
        }

        /// <summary>
        /// 生成QR
        /// </summary>
        /// <param name="text">QR文本</param>
        /// <param name="width">宽度</param>
        /// <param name="height">长度</param>
        /// <returns></returns>
        public static BitmapSource GenerateQRCode(string text, int width = 300, int height = 300)
        {
            ZXing.Windows.Compatibility.BarcodeWriter writer = new();
            writer.Format = ZXing.BarcodeFormat.QR_CODE;
            ZXing.QrCode.QrCodeEncodingOptions options = new()
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = width,
                Height = height,
                Margin = 1
            };
            writer.Options = options;
            var bitmap = writer.Write(text);
            return Convert2BitmapSource(bitmap);
        }

        /// <summary>
        /// Bitmap转BitmapSource
        /// </summary>
        /// <param name="source">Bitmap数据</param>
        /// <returns>BitmapSource数据</returns>
        public static BitmapSource Convert2BitmapSource(Bitmap source)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        /// <summary>
        /// BitmapSource转Bitmap
        /// </summary>
        /// <param name="bitmapsource">BitmapSource数据</param>
        /// <returns>Bitmap数据</returns>
        public static Bitmap Convert2Bitmap(BitmapSource bitmapSource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
    }
}
