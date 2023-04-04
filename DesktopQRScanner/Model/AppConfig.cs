﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopQRScanner.Model
{
    internal class AppConfig
    {
        /// <summary>
        /// 配置文件存储路径
        /// </summary>
        public static readonly string SavePath = $"{AppDomain.CurrentDomain.BaseDirectory}DesktopQRScannerConfig.json";

        /// <summary>
        /// 自动打开链接
        /// </summary>
        public bool AutoOpenLink { get; set; } = true;

        /// <summary>
        /// 历史记录数量
        /// </summary>
        public int HistorySaveCount { get; set; } = 100;
    }
}
