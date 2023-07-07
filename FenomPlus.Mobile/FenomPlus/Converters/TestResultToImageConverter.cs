using System;
using Xamarin.Forms;

namespace FenomPlus.Converters
{
    internal class TestResultToImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var testResult = value.ToString();
            switch (testResult)
            {
                case "Pass":
                    return ImageSource.FromFile("QualityControlFull.png");
                case "Fail":
                    return ImageSource.FromFile("quality_control_red.png");
                default:
                    return null;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
