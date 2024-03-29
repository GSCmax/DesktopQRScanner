using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DesktopQRScanner.Model
{
    internal partial class LinkItem : ObservableObject
    {
        [ObservableProperty]
        private string link;

        [ObservableProperty]
        private bool isStared;

        [ObservableProperty]
        private DateTime linkDateTime;
    }
}
