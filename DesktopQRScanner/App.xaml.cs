using DesktopQRScanner.Tools;
using DesktopQRScanner.VModel;
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

            if (GlobalDataHelper.appConfig.AutoFullScreenShotOnStart)
                GlobalDataHelper.bs = Screenshot.CaptureAllScreens();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.Save();
        }
    }
}
