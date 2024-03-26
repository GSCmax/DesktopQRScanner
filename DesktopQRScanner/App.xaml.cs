using DesktopQRScanner.Tools;
using DesktopQRScanner.VModel;
using HandyControl.Data;
using HandyControl.Tools;
using HinsHo.ScreenShot.CSharp;
using System;
using System.Windows;
using System.Windows.Media;

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
                var mainWindowDataContext = Current.MainWindow?.DataContext as MainWindowVModel;
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

            if (GlobalDataHelper.appConfig.PrimaryColor.ToUpper() != "SYSTEM")
            {
                try
                {
                    Color tempColor = (Color)ColorConverter.ConvertFromString(GlobalDataHelper.appConfig.PrimaryColor);
                }
                catch
                {
                    GlobalDataHelper.appConfig.PrimaryColor = (Application.Current.Resources["PrimaryBrush"] as SolidColorBrush).Color.ToString();
                }
            }
            GlobalDataHelper.Save();
        }

        internal void UpdateSkin(SkinType skin)
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
        }
    }
}
