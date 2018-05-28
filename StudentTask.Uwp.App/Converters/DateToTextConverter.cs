using System;
using Windows.UI.Xaml.Data;

namespace StudentTask.Uwp.App.Converters
{
    public class DateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var date = (DateTimeOffset) value;
            return date.Day + "." + date.Month + "." + date.Year;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}