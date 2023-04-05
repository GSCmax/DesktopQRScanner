using DesktopQRScanner.Model;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace DesktopQRScanner.Tools
{
    internal class GlobalDataHelper
    {
        /// <summary>
        /// 存储当前App实例的配置信息
        /// </summary>
        public static AppConfig appConfig;

        /// <summary>
        /// 存储当前App实例接受的消息
        /// </summary>
        public static BindingList<LinkItem> historyLinks;

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
            if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}HistoryLinks.json"))
                try
                {
                    var json = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}HistoryLinks.json");
                    historyLinks = (string.IsNullOrEmpty(json) ? new BindingList<LinkItem>() : JsonConvert.DeserializeObject<BindingList<LinkItem>>(json)) ?? new BindingList<LinkItem>();
                }
                catch
                {
                    historyLinks = new BindingList<LinkItem>();
                }
            else
                historyLinks = new BindingList<LinkItem>();
        }


        /// <summary>
        /// 保存本地存储的配置信息
        /// </summary>
        public static void Save()
        {
            var json1 = JsonConvert.SerializeObject(appConfig, Formatting.Indented);
            File.WriteAllText(AppConfig.SavePath, json1);

            var json2 = JsonConvert.SerializeObject(appConfig.AutoArrange ? historyLinks.OrderByDescending(n => n.IsStared).Take(appConfig.HistorySaveCount) : historyLinks.Take(appConfig.HistorySaveCount), Formatting.Indented);
            File.WriteAllText($"{AppDomain.CurrentDomain.BaseDirectory}HistoryLinks.json", json2);
        }
    }
}
