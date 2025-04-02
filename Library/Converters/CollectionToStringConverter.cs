using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Globalization;

namespace Library.Converters
{
    public class CollectionToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable items)
            {
                return string.Join(", ", items.Cast<object>());
            }
            return string.Empty;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}