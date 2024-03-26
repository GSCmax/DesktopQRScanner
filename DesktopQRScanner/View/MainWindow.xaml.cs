using DesktopQRScanner.Model;
using DesktopQRScanner.VModel;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DesktopQRScanner.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : HandyControl.Controls.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                HandlePaste();
                e.Handled = true; //阻止默认粘贴操作
            }
        }

        private void Paste_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            HandlePaste();
        }

        private void HandlePaste()
        {
            if (Clipboard.ContainsImage())
            {
                try
                {
                    var temp = Clipboard.GetImage();
                    if (temp != null)
                    {
                        (DataContext as MainWindowVModel).BitmapSource4Binding = new BitmapSource4BindingClass()
                        {
                            NeedRaise = true,
                            BitmapSourceData = new FormatConvertedBitmap(temp, PixelFormats.Bgr32, null, 0)
                        };
                    }
                }
                catch
                {
                    (DataContext as MainWindowVModel).ErrMsg = "无法粘贴此图像";
                }
            }
            else if (Clipboard.ContainsText())
            {
                try
                {
                    var temp = Clipboard.GetText();
                    if (temp != null)
                    {
                        (DataContext as MainWindowVModel).BitmapSource4Binding = new BitmapSource4BindingClass()
                        {
                            NeedRaise = false,
                            BitmapSourceData = Tools.ZXingHelper.GenerateQRCode(temp),
                            BitmapSourceString = temp
                        };
                    }
                }
                catch
                {
                    (DataContext as MainWindowVModel).ErrMsg = "无法粘贴此文本";
                }
            }
            else if (Clipboard.ContainsFileDropList())
            {
                try
                {
                    var temp = Clipboard.GetFileDropList();
                    string file = temp[0];
                    string extension = System.IO.Path.GetExtension(file).ToLower();
                    switch (extension)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".bmp":
                        case ".webp":
                            (DataContext as MainWindowVModel).BitmapSource4Binding = new BitmapSource4BindingClass()
                            {
                                NeedRaise = true,
                                BitmapSourceData = new BitmapImage(new Uri(file))
                            };
                            break;
                        default:
                            (DataContext as MainWindowVModel).ErrMsg = "无法粘贴此文件";
                            break;
                    }
                }
                catch
                {
                    (DataContext as MainWindowVModel).ErrMsg = "无法粘贴此文件";
                }
            }
            else
            {
                (DataContext as MainWindowVModel).ErrMsg = "无法粘贴此内容";
            }
        }

        private void imageFile_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
                string extension = System.IO.Path.GetExtension(file).ToLower();
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".bmp":
                    case ".webp":
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        break;
                    default:
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                        break;
                }
            }
        }

        private void imageFile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
                string extension = System.IO.Path.GetExtension(file).ToLower();
                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".bmp":
                    case ".webp":
                        try
                        {
                            (DataContext as MainWindowVModel).BitmapSource4Binding = new BitmapSource4BindingClass()
                            {
                                NeedRaise = true,
                                BitmapSourceData = new BitmapImage(new Uri(file))
                            };
                        }
                        catch
                        {
                            (DataContext as MainWindowVModel).ErrMsg = "无法从流中读取";
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (primaryColorTB.Text.ToUpper() == "SYSTEM")
            {
                Application.Current.Resources["PrimaryBrush"] = SystemParameters.WindowGlassBrush;
                SystemParameters.StaticPropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SystemParameters.WindowGlassBrush))
                    {
                        Application.Current.Resources["PrimaryBrush"] = SystemParameters.WindowGlassBrush;
                    }
                };
                (DataContext as MainWindowVModel).ErrMsg = null;
            }
            else
            {
                try
                {
                    Application.Current.Resources["PrimaryBrush"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(primaryColorTB.Text));
                    (DataContext as MainWindowVModel).ErrMsg = null;
                }
                catch
                {
                    (DataContext as MainWindowVModel).ErrMsg = "颜色代码无效";
                }
            }
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            PreviewKeyDown -= Window_PreviewKeyDown;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            PreviewKeyDown += Window_PreviewKeyDown;
        }
    }

    /// <summary>
    /// 旋转二维码展示区打开图标
    /// </summary>
    public class ImageRotateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            else
                return 45;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 旋转二维码展示区打开图标
    /// </summary>
    public class ImageToolTipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "拖拽图像至此处或点击选择图像文件以打开";
            else
                return "拖拽图像至此处或点击清除";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 旋转二维码展示区保存图标（未使用）
    /// </summary>
    public class SaveBTNShowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 文本是否为空转换器
    /// </summary>
    public class String2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// xaml字符串重复（未使用）
    /// </summary>
    public class StringRepeatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                var builder = new StringBuilder();
                int num;
                if (parameter is string numStr)
                {
                    if (!int.TryParse(numStr, out num))
                    {
                        return strValue;
                    }
                }
                else if (parameter is int intValue)
                {
                    num = intValue;
                }
                else
                {
                    return strValue;
                }
                for (var i = 0; i < num; i++)
                {
                    builder.Append(strValue + " ");
                }
                return builder.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// listbox模板选择器（未使用）
    /// </summary>
    public class MyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UnselectedTemplate { get; set; }
        public DataTemplate SelectedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var ele = container as FrameworkElement;
            var li = item as LinkItem;
            return li.IsSelected ? SelectedTemplate : UnselectedTemplate;
        }
    }
}
