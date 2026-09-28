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
        return typeInfo.IsGenericType && IsCurrentGenericDefinition(targetType, genericType) || typeInfo.BaseType is not null && typeInfo.BaseType.IsGenericDefinition(genericType);
    }

    private static bool IsCurrentGenericDefinition(Type targetType, Type genericType)
    {
        TypeInfo typeInfo = genericType.GetTypeInfo();
        return !typeInfo.IsInterface
            ? (object)targetType.GetGenericTypeDefinition() == genericType
            : HasGenericInterface(targetType, genericType);
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
