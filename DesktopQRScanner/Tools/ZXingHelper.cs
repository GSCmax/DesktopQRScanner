﻿using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;

namespace DesktopQRScanner.Tools
{
    internal class ZXingHelper
    {
        public static string ReadQRCode(ImageSource imageSource)
        {
            var reader = new BarcodeReader();
            var result = reader.Decode(Convert2Bitmap((BitmapSource)imageSource));
            return result?.Text;
        }


        public static ImageSource GenerateQRCode(string text, int width = 300, int height = 300)
        {
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            QrCodeEncodingOptions options = new QrCodeEncodingOptions()
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
        public static Bitmap Convert2Bitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }
    }
}
