using System;
using System.Reflection;

namespace Mokus2D.Util;

public static class ReflectionHelperExtensions
{
	public static bool IsInstanceOfType(this Type type, object instance)
	{
		return type.GetTypeInfo().IsAssignableFrom(instance.GetType().GetTypeInfo());
	}

	public static ReflectionHelper Reflect(this object target)
	{
		return ReflectionHelper.For(target);
	}

	public static bool IsGenericDefinition(this Type targetType, Type genericType)
	{
		TypeInfo typeInfo = targetType.GetTypeInfo();
		if (typeInfo.IsGenericType && IsCurrentGenericDefinition(targetType, genericType))
		{
			return true;
		}
		if ((object)typeInfo.BaseType != null)
		{
			return typeInfo.BaseType.IsGenericDefinition(genericType);
		}
		return false;
	}

	private static bool IsCurrentGenericDefinition(Type targetType, Type genericType)
	{
		TypeInfo typeInfo = genericType.GetTypeInfo();
		if (!typeInfo.IsInterface)
		{
			return (object)targetType.GetGenericTypeDefinition() == genericType;
		}
		return HasGenericInterface(targetType, genericType);
	}

	private static bool HasGenericInterface(Type targetType, Type genericInterface)
	{
		TypeInfo typeInfo = targetType.GetTypeInfo();
		foreach (Type implementedInterface in typeInfo.ImplementedInterfaces)
		{
			TypeInfo typeInfo2 = implementedInterface.GetTypeInfo();
			if (typeInfo2.IsGenericType && (object)implementedInterface.GetGenericTypeDefinition() == genericInterface)
			{
				return true;
			}
		}
		return false;
	}
}
