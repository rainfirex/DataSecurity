using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataSecurity.Modules
{
    class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return Binding.DoNothing;
        }
    }
}
