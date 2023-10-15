using CommunityToolkit.Mvvm.ComponentModel;

namespace DesktopQRScanner.Model
{
    internal partial class WebCamItem : ObservableObject
    {
        [ObservableProperty]
        private int camIndex;

        [ObservableProperty]
        private string camName;
    }
}
