namespace Neko.Utility.Core.Configurations
{
    public class GenerateCodeConfiguration
    {
        /// <summary>
        /// 二维码/条形码的边距
        /// </summary>
        public int CodeMargin { get; set; }

        /// <summary>
        /// 二维码/条形码的长
        /// </summary>
        public int CodeWidth { get; set; }

        /// <summary>
        /// 二维码/条形码的宽
        /// </summary>
        public int CodeHeight { get; set; }

        /// <summary>
        /// 二维码logo的长
        /// <para>仅在生成二维码时有效</para>
        /// </summary>
        public int LogoWidth { get; set; }

        /// <summary>
        /// 二维码logo的宽
        /// <para>仅在生成二维码时有效</para>
        /// </summary>
        public int LogoHeight { get; set; }

        /// <summary>
        /// 二维码/条形码的内容编码
        /// </summary>
        public string CodeCharset { get; set; }

        /// <summary>
        /// 生成二维码的默认配置
        /// </summary>
        public static GenerateCodeConfiguration QrCodeDefault { get; }

        /// <summary>
        /// 生成条形码的默认配置
        /// </summary>
        public static GenerateCodeConfiguration BarCodeDefault { get; }

        static GenerateCodeConfiguration()
        {
            QrCodeDefault = InitQrCodeDefault();
            BarCodeDefault = InitBarCodeDefault();
        }

        /// <summary>
        /// 初始化条形码默认配置
        /// </summary>
        /// <returns></returns>
        private static GenerateCodeConfiguration InitBarCodeDefault()
        {
            return new GenerateCodeConfiguration()
            {
                CodeMargin = 1,
                CodeWidth = 100,
                CodeHeight = 40,
                CodeCharset = "UTF-8"
            };
        }

        /// <summary>
        /// 初始化二维码默认配置
        /// </summary>
        /// <returns></returns>
        private static GenerateCodeConfiguration InitQrCodeDefault()
        {
            return new GenerateCodeConfiguration()
            {
                CodeMargin = 1,
                CodeWidth = 250,
                CodeHeight = 250,
                LogoWidth = 83,
                LogoHeight = 83,
                CodeCharset = "UTF-8"
            };
        }
    }
}
