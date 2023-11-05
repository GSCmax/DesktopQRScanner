using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.Model
{
    internal partial class BitmapSource4BindingClass : ObservableObject
    {
        [ObservableProperty]
        private BitmapSource bitmapSourceData;

        [ObservableProperty]
        private bool needRaise;

        [ObservableProperty]
        private string bitmapSourceString;
    }
}
