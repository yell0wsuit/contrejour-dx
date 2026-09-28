using System;
using System.Globalization;

namespace Mokus2D.Util.Extensions;

public static class ConversionExtensions
{
    public static T ConvertToType<T>(this object source)
    {
        return source.ConvertToType<T>(CultureInfo.InvariantCulture.NumberFormat);
    }

    public static T ConvertToType<T>(this object source, IFormatProvider formatProvider)
    {
        Type typeFromHandle = typeof(T);
        return ConvertToType<T>(source, typeFromHandle, formatProvider);
    }

    private static T ConvertToType<T>(object source, Type type, IFormatProvider formatProvider)
    {
        return type.IsGenericDefinition(typeof(Nullable<>))
            ? source == null ? (T)(object)null : (T)Convert.ChangeType(source, Nullable.GetUnderlyingType(type), formatProvider)
            : (T)Convert.ChangeType(source, type, formatProvider);
    }

    public static object ConvertToType(this object source, Type type)
    {
        return source.ConvertToType(type, CultureInfo.InvariantCulture.NumberFormat);
    }

    public static object ConvertToType(this object source, Type type, IFormatProvider formatProvider)
    {
        return type.IsGenericDefinition(typeof(Nullable<>))
            ? source == null ? null : Convert.ChangeType(source, Nullable.GetUnderlyingType(type), formatProvider)
            : Convert.ChangeType(source, type, formatProvider);
    }
}
