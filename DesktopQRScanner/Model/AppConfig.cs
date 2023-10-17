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
        /// 使用暗色主题
        /// </summary>
        public bool UseDarkTheme { get; set; } = false;

        /// <summary>
        /// 自动打开链接
        /// </summary>
        public bool AutoOpenLink { get; set; } = true;

        /// <summary>
        /// 历史自动整理
        /// </summary>
        public bool AutoArrange { get; set; } = true;

        /// <summary>
        /// 启动时自动截取全屏
        /// </summary>
        public bool AutoFullScreenShotOnStart { get; set; } = true;

        /// <summary>
        /// 历史记录数量
        /// </summary>
        public int HistorySaveCount { get; set; } = 50;

        /// <summary>
        /// 等待最小化动画完成时间
        /// </summary>
        public double MinimizeWaitDelay { get; set; } = 0.2;

        /// <summary>
        /// 调用摄像头间隔时间
        /// </summary>
        public int UseWebCamDelay { get; set; } = 5;

        /// <summary>
        /// 使用哪个摄像头
        /// </summary>
        public int UseWebCamIndex { get; set; } = -1;
    }
}
