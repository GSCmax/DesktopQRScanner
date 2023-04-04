using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopQRScanner.VModel
{
    internal partial class MainWindowVModel : ObservableObject
    {
        [ObservableProperty]
        private bool showPop = false;

        [RelayCommand]
        private void showPopClick()
        {
            ShowPop = true;
        }
    }
}
