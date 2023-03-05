using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System;
using Avalonia;

namespace EmpowerIDTest.Client.Utils;

public class IsEqualConverter : MarkupExtension, IValueConverter
{
    public virtual object? WhenEqual { get; set; } = true;

    public virtual object? WhenNotEqual { get; set; } = false;

    public virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Equals(value, parameter) ? WhenEqual : WhenNotEqual;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.Equals(true) == true ? parameter : AvaloniaProperty.UnsetValue;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}