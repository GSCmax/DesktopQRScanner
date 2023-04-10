using DesktopQRScanner.Model;
using DesktopQRScanner.VModel;
using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                        e.Effects = DragDropEffects.Copy;
                        e.Handled = true;
                        break;
                    default:
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
    }

    /// <summary>
    /// 旋转二维码展示区打开图标
    /// </summary>
    public class ImageRotateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 45;
            else
                return 0;
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
                return "点击打开图像文件";
            else
                return "点击清除";
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
