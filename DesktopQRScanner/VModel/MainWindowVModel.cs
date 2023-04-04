using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DesktopQRScanner.Model;
using DesktopQRScanner.Tools;
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

        [RelayCommand]
        private void openLink(object o)
        {

        }

        [RelayCommand]
        private void toggleStared(LinkItem o)
        {
            o.IsStared = !o.IsStared;
        }

        [RelayCommand]
        private void removeLink(LinkItem o)
        {
            GlobalDataHelper.historyLinks.Remove(o);
        }
    }
}
