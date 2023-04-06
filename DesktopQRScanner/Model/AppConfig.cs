using System;

namespace DesktopQRScanner.Model
{
    internal class AppConfig
    {
        /// <summary>
        /// 配置文件存储路径
        /// </summary>
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}Config.json";

        /// <summary>
        /// 自动打开链接
        /// </summary>
        public bool AutoOpenLink { get; set; } = true;

        /// <summary>
        /// 历史自动整理
        /// </summary>
        public bool AutoArrange { get; set; } = true;

        /// <summary>
        /// 历史记录数量
        /// </summary>
        public int HistorySaveCount { get; set; } = 100;
    }
}
