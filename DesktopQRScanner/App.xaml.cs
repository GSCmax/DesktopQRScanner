using DesktopQRScanner.Tools;
using DesktopQRScanner.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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

            GlobalDataHelper.Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            GlobalDataHelper.Save();
        }
    }
}
