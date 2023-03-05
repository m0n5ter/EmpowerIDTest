using System;
using System.Globalization;

namespace EmpowerIDTest.Client.Utils;

public class IsNullConverter : IsEqualConverter
{
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? WhenEqual : WhenNotEqual;
    }
}