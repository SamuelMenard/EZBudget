using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace EZBudget.ValueConverter
{
    class HexToBrushConverter : IValueConverter
    {
        public static HexToBrushConverter instance = new HexToBrushConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = (string)value;
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }
}
