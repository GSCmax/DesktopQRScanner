﻿using DesktopQRScanner.Tools;
using DesktopQRScanner.VModel;
using HandyControl.Data;
using HandyControl.Tools;
using HinsHo.ScreenShot.CSharp;
using System.Windows;

namespace DesktopQRScanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += (s, e) =>
            {
                var mainWindowDataContext = Application.Current.MainWindow.DataContext as MainWindowVModel;
                mainWindowDataContext.ErrMsg = e.Exception.Message;

                e.Handled = true;
            };

            GlobalDataHelper.Init();

            if (GlobalDataHelper.appConfig.UseDarkTheme)
                UpdateSkin(SkinType.Dark);

            if (GlobalDataHelper.appConfig.AutoFullScreenShotOnStart)
                GlobalDataHelper.bs = Screenshot.CaptureAllScreens();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.Save();
        }

        internal void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.RemoveAt(0);
            Resources.MergedDictionaries.Insert(0, ResourceHelper.GetSkin(skin));

            Current.MainWindow?.OnApplyTemplate();
        }
    }
}
