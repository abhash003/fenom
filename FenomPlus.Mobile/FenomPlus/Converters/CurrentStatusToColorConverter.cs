using FenomPlus.QualityControl.Enums;
using System;
using Xamarin.Forms;

namespace FenomPlus.Converters
{
    internal class CurrentStatusToBoolConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            string currentStatus = value.ToString();
            return currentStatus == "Qualified";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class CurrentStatusToBoolConverter1: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return false;
            string currentStatus = value.ToString();
            return currentStatus != "Qualified";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
