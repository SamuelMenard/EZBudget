using EZBudget.DataModels;
using EZBudget.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EZBudget.ValueConverter
{
    class ApplicationPageConverter : IValueConverter
    {
        public static ApplicationPageConverter instance = new ApplicationPageConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // find the appropriate page and returns it
            switch ((ApplicationPageEnum)value)
            {
                case ApplicationPageEnum.Login:
                    return new LoginPage();
                case ApplicationPageEnum.Main:
                    return new MainPage();
                default:
                    return new LoginPage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Object();
        }
    }
}
