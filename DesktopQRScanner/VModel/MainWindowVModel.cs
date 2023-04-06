using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopQRScanner.Model;
using DesktopQRScanner.Tools;
using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.VModel
{
    internal partial class MainWindowVModel : ObservableObject
    {
        public MainWindowVModel()
        {
            Screenshot.Snapped += Screenshot_Snapped;
            errTimer.Elapsed += ErrTimer_Elapsed;
        }

        /// <summary>
        /// 错误提示文字
        /// </summary>
        [ObservableProperty]
        private string errMsg = null;

        public Timer errTimer = new Timer()
        {
            AutoReset = false,
            Interval = 3000,
        };

        /// <summary>
        /// 清除错误提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ErrTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            ErrMsg = null;
        }

        /// <summary>
        /// Github按钮点击
        /// </summary>
        [RelayCommand]
        private void showGithubClick()
        {
            Process.Start(new ProcessStartInfo() { FileName = @"https://github.com/GSCmax/DesktopQRScanner", UseShellExecute = true });
        }

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

        /// <summary>
        /// Image绑定图像
        /// </summary>
        [ObservableProperty]
        private ImageSource imageSource4Binding = null;

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
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)ImageSource4Binding));
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            }
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
                throw new Exception("无法打开此链接");
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        [RelayCommand]
        private void genLink()
        {
            ImageSource4Binding = ZXingHelper.GenerateQRCode(SelectedLinkItem.Link);
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
        /// 截图成功
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Screenshot_Snapped(object? sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.ImageSource> e)
        {
            ImageSource4Binding = e.Info;
            AddQRCode2List();
        }

        /// <summary>
        /// 打开图像文件
        /// </summary>
        [RelayCommand]
        private void openFileClick()
        {
            if (ImageSource4Binding == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "图像文件 (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == true)
                {
                    ImageSource4Binding = new BitmapImage(new Uri(openFileDialog.FileName));
                    AddQRCode2List();
                }
            }
            else
                ImageSource4Binding = null;
        }

        /// <summary>
        /// QR扫描
        /// </summary>
        private void AddQRCode2List()
        {
            string t = ZXingHelper.ReadQRCode(ImageSource4Binding);
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
                throw new Exception("未能识别二维码");
            }
        }
    }
}
