using DesktopQRScanner.Tools;
using DesktopQRScanner.VModel;
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

            DispatcherUnhandledException += App_DispatcherUnhandledException;

            GlobalDataHelper.Init();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var mainWindowDataContext = Application.Current.MainWindow.DataContext as MainWindowVModel;
            mainWindowDataContext.ErrMsg = e.Exception.Message;
            mainWindowDataContext.errTimer.Start();

            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.Save();
        }
    }
}
