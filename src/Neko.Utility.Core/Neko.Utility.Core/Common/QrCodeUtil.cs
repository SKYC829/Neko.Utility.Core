using Neko.Utility.Core.Configurations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode.Internal;

namespace Neko.Utility.Core.Common
{
    /// <summary>
    /// 二维码/条形码帮助类
    /// <para>可以生成和读取二维码或条形码</para>
    /// </summary>
    public sealed class QrCodeUtil
    {
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">要生成的二维码的内容</param>
        /// <param name="configuration">生成二维码的配置<para>为空时使用<see cref="GenerateCodeConfiguration.QrCodeDefault"/></para></param>
        /// <returns></returns>
        public static Bitmap GenerateQrCode(string content, GenerateCodeConfiguration configuration = null)
        {
            return GenerateQrCode(content, string.Empty, configuration);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">要生成的二维码的内容</param>
        /// <param name="logoPath">二维码上图标文件的路径(为空时则不会生成图标)</param>
        /// <param name="configuration">生成二维码的配置<para>为空时使用<see cref="GenerateCodeConfiguration.QrCodeDefault"/></para></param>
        /// <returns></returns>
        public static Bitmap GenerateQrCode(string content, string logoPath, GenerateCodeConfiguration configuration = null)
        {
            Bitmap logo = null;
            if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
            {
                logo = new Bitmap(logoPath);
            }
            return GenerateQrCode(content, logo, configuration);
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="content">要生成的二维码的内容</param>
        /// <param name="logo">二维码上图标的<see cref="Bitmap"/>资源(为空时则不会生成图标)</param>
        /// <param name="configuration">生成二维码的配置<para>为空时使用<see cref="GenerateCodeConfiguration.QrCodeDefault"/></para></param>
        /// <returns></returns>
        public static Bitmap GenerateQrCode(string content, Bitmap logo, GenerateCodeConfiguration configuration = null)
        {
            if (configuration == null)
            {
                configuration = GenerateCodeConfiguration.QrCodeDefault;
            }
            MultiFormatWriter formatWriter = new MultiFormatWriter();
            Dictionary<EncodeHintType, object> configs = new Dictionary<EncodeHintType, object>()
            {
                {EncodeHintType.CHARACTER_SET,configuration.CodeCharset},
                {EncodeHintType.ERROR_CORRECTION,ErrorCorrectionLevel.H},
                {EncodeHintType.MARGIN,configuration.CodeMargin},
                {EncodeHintType.DISABLE_ECI,true}
            };
            BitMatrix matrix = formatWriter.encode(content, BarcodeFormat.QR_CODE, configuration.CodeWidth, configuration.CodeHeight, configs);
            BarcodeWriter codeWriter = new BarcodeWriter();
            Bitmap result = codeWriter.Write(matrix);
            if (logo != null)
            {
                result = DrawCodeLogo(codeBitmap: result, matrixRectangle: matrix.getEnclosingRectangle(), codeLogo: logo, configuration: configuration);
            }
            return result;
        }

        /// <summary>
        /// 绘制二维码logo
        /// </summary>
        /// <param name="codeBitmap">要绘制logo的二维码<see cref="Bitmap"/>资源</param>
        /// <param name="matrixRectangle">要绘制logo的二维码<see cref="Bitmap"/>资源的矩阵信息
        /// <para>
        /// <list type="bullet">
        /// <item>矩阵第0位:左边距</item>
        /// <item>矩阵第1位:上边距</item>
        /// <item>矩阵第2位:二维码的宽</item>
        /// <item>矩阵第0位:二维码的高</item>
        /// </list>
        /// </para>
        /// </param>
        /// <param name="codeLogo">logo的<see cref="Bitmap"/>资源</param>
        /// <param name="configuration">生成二维码/条码的配置</param>
        /// <returns></returns>
        public static Bitmap DrawCodeLogo(Bitmap codeBitmap, int[] matrixRectangle, Bitmap codeLogo, GenerateCodeConfiguration configuration)
        {
            if (codeBitmap == null)
            {
                return null;
            }

            if (configuration == null)
            {
                return codeBitmap;
            }

            if (matrixRectangle == null || matrixRectangle.Length < 4)
            {
                return codeBitmap;
            }

            if (configuration.LogoWidth <= 0)
            {
                configuration.LogoWidth = codeLogo.Width;
            }

            if (configuration.LogoHeight <= 0)
            {
                configuration.LogoHeight = codeLogo.Height;
            }

            if (codeLogo == null)
            {
                return codeBitmap;
            }
            configuration.LogoWidth = Math.Min(configuration.LogoWidth, (int)matrixRectangle[2] / 3);
            configuration.LogoHeight = Math.Min(configuration.LogoHeight, (int)matrixRectangle[3] / 3);
            int logoLeft = codeBitmap.Width - configuration.LogoWidth;
            int logoTop = codeBitmap.Height - configuration.LogoHeight;
            Bitmap result = new Bitmap(codeBitmap.Width, codeBitmap.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(codeBitmap, 0, 0, codeBitmap.Width, codeBitmap.Height);
                graphics.FillRectangle(Brushes.White, logoLeft, logoTop, configuration.LogoWidth, configuration.LogoHeight);
                graphics.DrawImage(codeLogo, logoLeft, logoTop, configuration.LogoWidth, configuration.LogoHeight);
            }
            return result;
        }

        /// <summary>
        /// 生成条形码
        /// </summary>
        /// <param name="content">要生成的条形码的内容</param>
        /// <param name="configuration">生成条形码的配置<para>为空时使用<see cref="GenerateCodeConfiguration.BarCodeDefault"/></para></param>
        /// <returns></returns>
        public static Bitmap GenerateBarCode(string content, GenerateCodeConfiguration configuration = null)
        {
            if (configuration == null)
            {
                configuration = GenerateCodeConfiguration.BarCodeDefault;
            }
            BarcodeWriter codeWriter = new BarcodeWriter();
            codeWriter.Format = BarcodeFormat.CODE_128;
            codeWriter.Options = new EncodingOptions()
            {
                Height = configuration.CodeHeight,
                Width = configuration.CodeWidth,
                Margin = configuration.CodeMargin,
            };
            codeWriter.Options.Hints[EncodeHintType.CHARACTER_SET] = configuration.CodeCharset;
            Bitmap result = codeWriter.Write(content);
            return result;
        }

        /// <summary>
        /// 读取二维码/条形码
        /// </summary>
        /// <param name="imagePath">二维码/条形码图片路径</param>
        /// <returns></returns>
        public static string ReadCode(string imagePath, string charset = "UTF-8")
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                return string.Empty;
            }
            Bitmap codeImage = new Bitmap(imagePath);
            return ReadCode(codeImage, charset);
        }

        /// <summary>
        /// 读取二维码/条形码
        /// </summary>
        /// <param name="codeImage">二维码/条形码的<see cref="Bitmap"/>资源</param>
        /// <returns></returns>
        public static string ReadCode(Bitmap codeImage, string charset = "UTF-8")
        {
            if (codeImage == null)
            {
                return string.Empty;
            }
            BarcodeReader codeReader = new BarcodeReader();
            codeReader.Options = new DecodingOptions()
            {
                CharacterSet = charset
            };
            Result result = codeReader.Decode(codeImage);
            return result.Text;
        }
    }
}
