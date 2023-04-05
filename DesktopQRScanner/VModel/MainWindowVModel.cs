using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopQRScanner.Model;
using DesktopQRScanner.Tools;
using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.VModel
{
    internal partial class MainWindowVModel : ObservableObject
    {
        public MainWindowVModel() => Screenshot.Snapped += Screenshot_Snapped;

        /// <summary>
        /// 截图成功通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Screenshot_Snapped(object? sender, HandyControl.Data.FunctionEventArgs<System.Windows.Media.ImageSource> e)
        {
            ImageSource4Binding = e.Info;
            AddQRCode2List();
        }

        [RelayCommand]
        private void showGithubClick()
        {
            Process.Start(new ProcessStartInfo() { FileName = @"https://github.com/GSCmax/DesktopQRScanner", UseShellExecute = true });
        }

        [ObservableProperty]
        private bool showPop = false;

        [RelayCommand]
        private void showPopClick()
        {
            ShowPop = true;
        }

        /// <summary>
        /// 当前选择的内容
        /// </summary>
        [ObservableProperty]
        private LinkItem selectedLinkItem;

        /// <summary>
        /// 复制
        /// </summary>
        [RelayCommand]
        private void copyLink()
        {
            Clipboard.SetText(SelectedLinkItem.Link);
        }

        /// <summary>
        /// 打开
        /// </summary>
        [RelayCommand]
        private void openLink()
        {
            Process.Start(new ProcessStartInfo() { FileName = SelectedLinkItem.Link, UseShellExecute = true });
        }

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

        [ObservableProperty]
        private ImageSource imageSource4Binding = null;

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

            }
        }
    }
}
