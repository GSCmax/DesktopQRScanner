﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopQRScanner.Model;
using DesktopQRScanner.Tools;
using HandyControl.Data;
using HinsHo.ScreenShot.CSharp;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Timer = System.Timers.Timer;

namespace DesktopQRScanner.VModel
{
    internal partial class MainWindowVModel : ObservableObject
    {
        public MainWindowVModel()
        {
            errTimer.Elapsed += (s, e) => ErrMsg = null;
            infoTimer.Elapsed += (s, e) => InfoMsg = null;
            openWebCamButtonEnabledTimer.Elapsed += (s, e) => OpenWebCamButtonEnabled = true;

            if (GlobalDataHelper.bs != null)
                BitmapSource4Binding = new BitmapSource4BindingClass()
                {
                    NeedRaise = true,
                    BitmapSourceData = GlobalDataHelper.bs
                };

            vCapture = new VideoCapture();
            bkgWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            bkgWorker.DoWork += Worker_DoWork;
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
        partial void OnErrMsgChanged(string value) { if (value != null) errTimer.Start(); }
        private Timer errTimer = new Timer()
        {
            AutoReset = false,
            Interval = 3000,
        };

        /// <summary>
        /// 提示文字
        /// </summary>
        [ObservableProperty]
        private string infoMsg = null;
        partial void OnInfoMsgChanged(string value) { if (value != null) infoTimer.Start(); }
        private Timer infoTimer = new Timer()
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

        /// <summary>
        /// 打开历史记录文件
        /// </summary>
        [RelayCommand]
        private void openHistoryFile()
        {
            try
            {
                Process.Start(new ProcessStartInfo() { FileName = $"{AppDomain.CurrentDomain.BaseDirectory}Historys.json", UseShellExecute = true });
            }
            catch
            {
                ErrMsg = "未找到历史记录";
            }
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
        private BitmapSource4BindingClass bitmapSource4Binding = null;
        partial void OnBitmapSource4BindingChanged(BitmapSource4BindingClass value)
        {
            if (BitmapSource4Binding != null)
            {
                if (BitmapSource4Binding.NeedRaise)
                {
                    string qrtext = ZXingHelper.ReadQRCode(BitmapSource4Binding.BitmapSourceData);
                    if (qrtext != null)
                    {
                        SelectedLinkItem = new LinkItem()
                        {
                            Link = qrtext,
                            LinkDateTime = DateTime.Now,
                        };
                        GlobalDataHelper.historyLinks.Insert(0, SelectedLinkItem);
                        if (GlobalDataHelper.appConfig.AutoOpenLink)
                            openLink();

                        //结束拍摄
                        if (vCapture.IsOpened())
                            openWebCamClickCommand.Execute(null);

                        InfoMsg = "成功识别二维码";
                    }
                }
                else
                {
                    if (BitmapSource4Binding.BitmapSourceString != null)
                    {
                        SelectedLinkItem = new LinkItem()
                        {
                            Link = BitmapSource4Binding.BitmapSourceString,
                            LinkDateTime = DateTime.Now,
                        };
                        GlobalDataHelper.historyLinks.Insert(0, SelectedLinkItem);
                        if (GlobalDataHelper.appConfig.AutoOpenLink)
                            openLink();
                    }
                }
            }
        }

        /// <summary>
        /// 主题切换
        /// </summary>
        /// <param name="isDark"></param>
        [RelayCommand]
        private void skinToggle(bool isDark)
        {
            (Application.Current as App).UpdateSkin(isDark ? SkinType.Dark : SkinType.Default);
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
                Clipboard.SetImage(BitmapSource4Binding.BitmapSourceData);
            }
            catch
            {
                ErrMsg = "系统剪切板异常";
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
                encoder.Frames.Add(BitmapFrame.Create(BitmapSource4Binding.BitmapSourceData));
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
        [RelayCommand(CanExecute = nameof(ShowAddButton))]
        private void screenShot()
        {
            MainWindowState = WindowState.Minimized;
            Thread.Sleep((int)(GlobalDataHelper.appConfig.MinimizeWaitDelay * 1000));
            ScreenshotOptions screenshotOptions = new ScreenshotOptions()
            {
                BackgroundOpacity = 0.5,
                SelectionRectangleBorderBrush = (System.Windows.Media.Brush)Application.Current.FindResource("PrimaryBrush")
            };
            var data = Screenshot.CaptureRegionToBitmapSource(screenshotOptions);
            if (data != null)
                BitmapSource4Binding = new BitmapSource4BindingClass()
                {
                    NeedRaise = true,
                    BitmapSourceData = data
                };
            MainWindowState = WindowState.Normal;
        }

        /// <summary>
        /// 全局截图
        /// </summary>
        [RelayCommand(CanExecute = nameof(ShowAddButton))]
        private void fullScreenShot()
        {
            MainWindowState = WindowState.Minimized;
            Thread.Sleep((int)(GlobalDataHelper.appConfig.MinimizeWaitDelay * 1000));
            BitmapSource4Binding = new BitmapSource4BindingClass()
            {
                NeedRaise = true,
                BitmapSourceData = Screenshot.CaptureAllScreens()
            };
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
                ErrMsg = "系统剪切板异常";
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
                ErrMsg = "无法打开此链接";
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        [RelayCommand]
        private void genLink()
        {
            BitmapSource4Binding = new BitmapSource4BindingClass()
            {
                NeedRaise = false,
                BitmapSourceData = ZXingHelper.GenerateQRCode(SelectedLinkItem.Link)
            };
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
                openFileDialog.Filter = "图像文件 (*.jpg, *.jpeg, *.png, *.bmp, *webp)|*.jpg;*.jpeg;*.png;*.bmp;*.webp";

                if (openFileDialog.ShowDialog() == true)
                    try
                    {
                        BitmapSource4Binding = new BitmapSource4BindingClass()
                        {
                            NeedRaise = true,
                            BitmapSourceData = new BitmapImage(new Uri(openFileDialog.FileName))
                        };
                    }
                    catch
                    {
                        ErrMsg = "无法从流中读取";
                    }
            }
            else
            {
                BitmapSource4Binding = null;
            }
        }

        private readonly VideoCapture vCapture;
        private readonly BackgroundWorker bkgWorker;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(screenShotCommand))]
        [NotifyCanExecuteChangedFor(nameof(fullScreenShotCommand))]
        private bool showAddButton = true;

        [ObservableProperty]
        private bool openWebCamButtonEnabled = true;
        private Timer openWebCamButtonEnabledTimer = new Timer()
        {
            AutoReset = false,
        };

        /// <summary>
        /// 打开或关闭摄像头
        /// </summary>
        [RelayCommand]
        private void openWebCamClick()
        {
            if (!vCapture.IsOpened())
            {
                if (GlobalDataHelper.appConfig.UseWebCamIndex != -1)
                {
                    vCapture.Open(GlobalDataHelper.appConfig.UseWebCamIndex, VideoCaptureAPIs.ANY);
                    if (vCapture.IsOpened() && !bkgWorker.IsBusy)
                    {
                        bkgWorker.RunWorkerAsync();
                        ShowAddButton = false;
                    }
                    else
                    {
                        vCapture.Release();
                        ErrMsg = "打开摄像头失败";
                    }
                }
                else
                {
                    ErrMsg = "请先选择摄像头";
                }
            }
            else
            {
                bkgWorker.CancelAsync();
                vCapture.Release();
                ShowAddButton = true;
                OpenWebCamButtonEnabled = false;
                openWebCamButtonEnabledTimer.Interval = GlobalDataHelper.appConfig.UseWebCamDelay * 1000;
                openWebCamButtonEnabledTimer.Start();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 0;
            while (!((BackgroundWorker)sender).CancellationPending)
            {
                using (var frameMat = vCapture.RetrieveMat())
                {
                    if (i == 15)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapSource4Binding = new BitmapSource4BindingClass()
                            {
                                NeedRaise = true,
                                BitmapSourceData = frameMat.ToBitmapSource()
                            };
                        });
                        i = 0;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapSource4Binding = new BitmapSource4BindingClass()
                            {
                                NeedRaise = false,
                                BitmapSourceData = frameMat.ToBitmapSource()
                            };
                        });
                        i++;
                    }
                }
            }
        }
    }

    internal partial class BitmapSource4BindingClass : ObservableObject
    {
        [ObservableProperty]
        private BitmapSource bitmapSourceData;

        [ObservableProperty]
        private bool needRaise;

        [ObservableProperty]
        private string bitmapSourceString;
    }
}
