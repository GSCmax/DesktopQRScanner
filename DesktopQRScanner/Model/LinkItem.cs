using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopQRScanner.Model
{
    internal partial class LinkItem : ObservableObject
    {
        [ObservableProperty]
        private DateTime linkDateTime;

        [ObservableProperty]
        private string link;

        [ObservableProperty]
        private bool isStared;
    }
}

//https://icon-icons.com/
//magick