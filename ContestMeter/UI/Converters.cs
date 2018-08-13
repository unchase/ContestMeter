using System;
using System.Windows.Data;
using System.Windows;

namespace ContestMeter.UI
{
    public static class Converters
    {
        private static BooleanToVisibilityConverter _visibilityConverter = new BooleanToVisibilityConverter();

        public static BooleanToVisibilityConverter Visibility { get { return _visibilityConverter; } }
        
        #region VisibilityConverter
        /// <summary>
        /// http://www.codeproject.com/KB/WPF/FriendlyEnums.aspx
        /// </summary>
        [ValueConversion(typeof(bool), typeof(Visibility))]
        public class BooleanToVisibilityConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (value != null && value is bool)
                {
                    var flag = (bool)value;
                    return flag ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                }

                return System.Windows.Visibility.Visible;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotSupportedException();
            }


        }
        #endregion

    }
}