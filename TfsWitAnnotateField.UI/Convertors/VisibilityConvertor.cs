using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace TfsWitAnnotateField.UI.Convertors
{
    class VisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                       CultureInfo culture)
        {
            if (value != null && value is bool)
            {
                var bValue = (bool)value;
                if (bValue)
                { return Visibility.Visible; }
                else
                {
                    return Visibility.Collapsed;
                }
                //var visibility = (Visibility)Enum.Parse(
                //     typeof(Visibility), parameter.ToString(), true);
                //if (bValue) return visibility;
                //return visibility ==
                //   Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                           CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
