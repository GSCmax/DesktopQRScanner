using DesktopQRScanner.Model;
using GitHub.secile.UsbCamera;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.Tools
{
    internal class GlobalDataHelper
    {
        /// <summary>
        /// 当前App的版本信息与调试信息
        /// </summary>
#if DEBUG
        private static bool IsDebugMode = true;
#else
        private static bool IsDebugMode = false;
#endif
        public static string Version = Assembly.GetEntryAssembly().GetName().Version.ToString() + (Debugger.IsAttached ? " Attach" : (IsDebugMode ? " Debug" : " Release"));

        /// <summary>
        /// 存储当前App实例启动截图
        /// </summary>
        public static BitmapSource bs = null;

        /// <summary>
        /// 存储当前App实例的配置信息
        /// </summary>
        public static AppConfig appConfig;

        /// <summary>
        /// 存储当前App实例的历史记录
        /// </summary>
        public static BindingList<LinkItem> historyLinks;

        /// <summary>
        /// 存储当前App实例获取的摄像头信息
        /// </summary>
        public static BindingList<WebCamItem> cameraArray;

        /// <summary>
        /// 指示当前App实例是否获取到摄像头
        /// </summary>
        public static bool ifHaveCamera = false;

        /// <summary>
        /// 获取本地存储的配置信息
        /// </summary>
        public static void Init()
        {
            //读取Config
            if (File.Exists(AppConfig.SavePath))
                try
                {
                    var json = File.ReadAllText(AppConfig.SavePath);
                    appConfig = (string.IsNullOrEmpty(json) ? new AppConfig() : JsonConvert.DeserializeObject<AppConfig>(json)) ?? new AppConfig();
                }
                catch
                {
                    appConfig = new AppConfig();
                }
            else
                appConfig = new AppConfig();

            //读取devCustoms
            if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}Historys.json"))
                try
                {
                    var json = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Historys.json");
                    historyLinks = (string.IsNullOrEmpty(json) ? new BindingList<LinkItem>() : JsonConvert.DeserializeObject<BindingList<LinkItem>>(json)) ?? new BindingList<LinkItem>();
                }
                catch
                {
                    historyLinks = new BindingList<LinkItem>();
                }
            else
                historyLinks = new BindingList<LinkItem>();

            //读取摄像头列表
            string[] devices = UsbCamera.FindDevices();
            if (devices.Length > 0)
            {
                cameraArray = new BindingList<WebCamItem>() { new WebCamItem() { CamIndex = -1, CamName = string.Empty } };
                for (int i = 0; i < devices.Length; i++)
                    cameraArray.Add(new WebCamItem() { CamIndex = i, CamName = devices[i] });
                ifHaveCamera = true;
            }
        }

        /// <summary>
        /// 保存本地存储的配置信息
        /// </summary>
        public static void Save()
        {
            var json1 = JsonConvert.SerializeObject(appConfig, Formatting.Indented);
            File.WriteAllText(AppConfig.SavePath, json1);

            var json2 = JsonConvert.SerializeObject(appConfig.AutoArrange ? historyLinks.OrderByDescending(n => n.IsStared).Take(appConfig.HistorySaveCount) : historyLinks.Take(appConfig.HistorySaveCount), Formatting.Indented);
            File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}Historys.json", json2);
        }

        /// <summary>
        /// 获取系统主题色
        /// </summary>
        [DllImport("dwmapi.dll")]
        private static extern void DwmGetColorizationColor(out int pcrColorization, out bool pfOpaqueBlend);
        public static Color GetSystemColor()
        {
            int color;
            DwmGetColorizationColor(out color, out _);
            return new Color()
            {
                A = (byte)(color >> 24),
                R = (byte)(color >> 16),
                G = (byte)(color >> 8),
                B = (byte)(color)
            };
        }
    }
}
