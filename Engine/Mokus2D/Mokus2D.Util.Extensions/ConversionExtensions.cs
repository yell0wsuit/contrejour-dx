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
		if (type.IsGenericDefinition(typeof(Nullable<>)))
		{
			if (source == null)
			{
				return (T)(object)null;
			}
			return (T)Convert.ChangeType(source, Nullable.GetUnderlyingType(type), formatProvider);
		}
		return (T)Convert.ChangeType(source, type, formatProvider);
	}

	public static object ConvertToType(this object source, Type type)
	{
		return source.ConvertToType(type, CultureInfo.InvariantCulture.NumberFormat);
	}

	public static object ConvertToType(this object source, Type type, IFormatProvider formatProvider)
	{
		if (type.IsGenericDefinition(typeof(Nullable<>)))
		{
			if (source == null)
			{
				return null;
			}
			return Convert.ChangeType(source, Nullable.GetUnderlyingType(type), formatProvider);
		}
		return Convert.ChangeType(source, type, formatProvider);
	}
}
