﻿using System;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DesktopQRScanner.Model;

namespace DesktopQRScanner.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Image2RotateConverter : IValueConverter
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
                    builder.Append(strValue);
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

    public class MyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UnselectedTemplate { get; set; }
        public DataTemplate SelectedTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if((item as LinkItem).IsSelected)
            {
                return SelectedTemplate;
            }
            else
            {
                return UnselectedTemplate;
            }
        }
    }
}
