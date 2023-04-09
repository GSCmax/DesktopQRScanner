using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopQRScanner.Model;
using DesktopQRScanner.Tools;
using HinsHo.ScreenShot.CSharp;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.VModel
{
    internal partial class MainWindowVModel : ObservableObject
    {
        public MainWindowVModel()
        {
            errTimer.Elapsed += (s, e) => ErrMsg = null;
            infoTimer.Elapsed += (s, e) => InfoMsg = null;

            if (GlobalDataHelper.bs != null)
                BitmapSource4Binding = GlobalDataHelper.bs;
        }

        /// <summary>
        /// 窗口状态
        /// </summary>
        [ObservableProperty]
        private WindowState mainWindowState;

        /// <summary>
        /// 错误提示文字
        /// </summary>
        [ObservableProperty]
        private string errMsg = null;
        partial void OnErrMsgChanged(string value) => errTimer.Start();

        public Timer errTimer = new Timer()
        {
            AutoReset = false,
            Interval = 3000,
        };

        /// <summary>
        /// 提示文字
        /// </summary>
        [ObservableProperty]
        private string infoMsg = null;
        partial void OnInfoMsgChanged(string value) => infoTimer.Start();

        public Timer infoTimer = new Timer()
        {
            AutoReset = false,
            Interval = 3000,
        };

        /// <summary>
        /// Github按钮点击
        /// </summary>
        [RelayCommand]
        private void showGithubClick()
        {
            Process.Start(new ProcessStartInfo() { FileName = @"https://github.com/GSCmax/DesktopQRScanner", UseShellExecute = true });
        }

        #region 设置Popup

        /// <summary>
        /// 设置Pop展开状态
        /// </summary>
        [ObservableProperty]
        private bool showPop = false;

        /// <summary>
        /// 展开设置Pop
        /// </summary>
        [RelayCommand]
        private void showPopClick()
        {
            ShowPop = true;
        }

        #endregion

        /// <summary>
        /// Image绑定图像
        /// </summary>
        [ObservableProperty]
        private BitmapSource bitmapSource4Binding = null;
        partial void OnBitmapSource4BindingChanged(BitmapSource value)
        {
            if (BitmapSource4Binding != null)
            {
                string t = ZXingHelper.ReadQRCode(BitmapSource4Binding);
                if (t != null)
                {
                    SelectedLinkItem = new LinkItem()
                    {
                        IsStared = false,
                        Link = t,
                        LinkDateTime = DateTime.Now,
                    };
                    GlobalDataHelper.historyLinks.Insert(0, SelectedLinkItem);
                    if (GlobalDataHelper.appConfig.AutoOpenLink)
                        openLink();
                }
                else
                {
                    InfoMsg = "未能识别二维码";
                }
            }
        }

        #region 二维码右键菜单

        /// <summary>
        /// 复制二维码
        /// </summary>
        [RelayCommand]
        private void copyImage()
        {
            try
            {
                Clipboard.SetImage(BitmapSource4Binding);
            }
            catch
            {
                throw new Exception("系统剪切板异常");
            }
        }

        /// <summary>
        /// 保存二维码
        /// </summary>
        [RelayCommand]
        private void saveImage()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "图像文件 (*.png)|*.png";

            if (saveFileDialog.ShowDialog() == true)
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(BitmapSource4Binding));
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        #endregion

        /// <summary>
        /// 截图
        /// </summary>
        [RelayCommand]
        private void screenShot()
        {
            MainWindowState = WindowState.Minimized;
            ScreenshotOptions screenshotOptions = new ScreenshotOptions()
            {
                BackgroundOpacity = 0.5,
                SelectionRectangleBorderBrush = (System.Windows.Media.Brush)Application.Current.FindResource("PrimaryBrush")
            };
            BitmapSource4Binding = Screenshot.CaptureRegionToBitmapSource(screenshotOptions);
            MainWindowState = WindowState.Normal;
        }

        /// <summary>
        /// 全局截图
        /// </summary>
        [RelayCommand]
        private void fullScreenShot()
        {
            MainWindowState = WindowState.Minimized;
            BitmapSource4Binding = Screenshot.CaptureAllScreens();
            MainWindowState = WindowState.Normal;
        }

        /// <summary>
        /// 当前历史记录选择的内容
        /// </summary>
        [ObservableProperty]
        private LinkItem selectedLinkItem;

        #region 历史记录列表右键菜单

        /// <summary>
        /// 复制
        /// </summary>
        [RelayCommand]
        private void copyLink()
        {
            try
            {
                Clipboard.SetText(SelectedLinkItem.Link);
            }
            catch
            {
                throw new Exception("系统剪切板异常");
            }
        }

        /// <summary>
        /// 打开
        /// </summary>
        [RelayCommand]
        private void openLink()
        {
            try
            {
                Process.Start(new ProcessStartInfo() { FileName = SelectedLinkItem.Link, UseShellExecute = true });
            }
            catch
            {
                InfoMsg = "无法打开此链接";
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        [RelayCommand]
        private void genLink()
        {
            BitmapSource4Binding = ZXingHelper.GenerateQRCode(SelectedLinkItem.Link);
        }

        /// <summary>
        /// 收藏
        /// </summary>
        [RelayCommand]
        private void toggleStared()
        {
            SelectedLinkItem.IsStared = !SelectedLinkItem.IsStared;
        }

        /// <summary>
        /// 删除
        /// </summary>
        [RelayCommand]
        private void removeLink()
        {
            GlobalDataHelper.historyLinks.Remove(SelectedLinkItem);
        }

        #endregion

        /// <summary>
        /// 打开图像文件
        /// </summary>
        [RelayCommand]
        private void openFileClick()
        {
            if (BitmapSource4Binding == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "图像文件 (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == true)
                    BitmapSource4Binding = new BitmapImage(new Uri(openFileDialog.FileName));
            }
            else
                BitmapSource4Binding = null;
        }
    }
}
