using System;
using System.Globalization;
using System.Reflection;

namespace Mokus2D.Util;

public static class ReflectUtil
{
    public static object CreateInstance(string typeName, Type mainAssemblyClass)
    {
        Type type = GetType(typeName, mainAssemblyClass);
        return Activator.CreateInstance(type);
    }

    public static object CreateInstance(Type mainAssemblyClass, string typeName, params object[] parameters)
    {
        Type type = GetType(typeName, mainAssemblyClass);
        return Activator.CreateInstance(type, parameters);
    }

    public static object SafeCreateInstance(string typeName, Type mainAssemblyClass)
    {
        Type type = GetType(typeName, mainAssemblyClass);
        return type is not null ? Activator.CreateInstance(type) : null;
    }

    private static Type GetType(string typeName, Type mainAssemblyClass)
    {
        Assembly assembly = mainAssemblyClass.GetTypeInfo().Assembly;
        return assembly.GetType(typeName);
    }

    public static object CreateInstance(string typeName)
    {
        Type type = Type.GetType(typeName);
        return Activator.CreateInstance(type);
    }

    public static object CreateInstance(string typeName, params object[] parameters)
    {
        Type type = Type.GetType(typeName);
        return Activator.CreateInstance(type, parameters);
    }

    public static object CreateInstance(Type type, params object[] parameters)
    {
        return Activator.CreateInstance(type, parameters);
    }

    public static void SetValue(object o, string name, object value)
    {
        FieldInfo field = ReflectionExtensions.GetField(o.GetType().GetTypeInfo(), name);
        if (field is not null)
        {
            value = Convert.ChangeType(value, field.FieldType, CultureInfo.InvariantCulture.NumberFormat);
            field.SetValue(o, value);
        }
        else
        {
            PropertyInfo property = ReflectionExtensions.GetProperty(o.GetType().GetTypeInfo(), name);
            value = Convert.ChangeType(value, property.PropertyType, CultureInfo.InvariantCulture.NumberFormat);
            property.SetValue(o, value, null);
        }
    }

    public static object InvokeMethod(object target, string name, params object[] arguments)
    {
        return target.GetType().GetTypeInfo().GetDeclaredMethod(name)
            .Invoke(target, arguments);
    }

    public static TypeInfo CastToTypeInfo(object obj)
    {
        return (TypeInfo)obj;
    }
}
