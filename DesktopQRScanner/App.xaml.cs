using DesktopQRScanner.Tools;
using DesktopQRScanner.View;
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
                mainWindowDataContext.errTimer.Start();
                e.Handled = true;
            };

            GlobalDataHelper.Init();

            var mw = new MainWindow(GlobalDataHelper.appConfig.AutoFullScreenShotOnStart ? Screenshot.CaptureAllScreens() : null);
            mw.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.Save();
        }
    }
}
