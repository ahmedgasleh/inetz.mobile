// Converters/NullOrEmptyToFalseConverter.cs
using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace inetz.ifinance.app.Converters
{
    public class NullOrEmptyToFalseConverter : IValueConverter
    {
        public object Convert ( object? value, Type targetType, object? parameter, CultureInfo culture )
            => !string.IsNullOrWhiteSpace(value as string);

        public object ConvertBack ( object? value, Type targetType, object? parameter, CultureInfo culture )
            => throw new NotImplementedException();
    }
}
