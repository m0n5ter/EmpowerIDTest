using System;
using System.Globalization;

namespace EmpowerIDTest.Client.Utils;

public class IsTrueOrFalseConverter : IsEqualConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return base.Convert(value, targetType, true, culture);
    }
}