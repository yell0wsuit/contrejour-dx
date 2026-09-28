using System;
using System.Globalization;
using System.Reflection;

namespace Mokus2D.Util;

public class ReflectionHelper
{
    private readonly object target;

    private MemberInfo currentMemberInfo;

    private ReflectionHelper(object target)
    {
        this.target = target ?? throw new ArgumentNullException("target");
    }

    public static FieldInfo FindField(Type type, string name)
    {
        if (type is null || (object)type == typeof(object))
        {
            return null;
        }
        TypeInfo typeInfo = type.GetTypeInfo();
        return typeInfo.GetDeclaredField(name) ?? FindField(typeInfo.BaseType, name);
    }

    public static PropertyInfo FindProperty(Type type, string name)
    {
        if (type is null || (object)type == typeof(object))
        {
            return null;
        }
        TypeInfo typeInfo = type.GetTypeInfo();
        return typeInfo.GetDeclaredProperty(name) ?? FindProperty(typeInfo.BaseType, name);
    }

    public static ReflectionHelper For(object targetInstance)
    {
        return new ReflectionHelper(targetInstance);
    }

    public ReflectionHelper Field(string fieldName)
    {
        currentMemberInfo = FindField(target.GetType(), fieldName);
        return currentMemberInfo is null
            ? throw new NullReferenceException(string.Format("Sorry, There is no such field = {0} in class type {1}", new object[2]
            {
                fieldName,
                target.GetType().FullName
            }))
            : this;
    }

    public ReflectionHelper FieldOrProperty(string fieldName)
    {
        currentMemberInfo = FindField(target.GetType(), fieldName);
        currentMemberInfo ??= FindProperty(target.GetType(), fieldName) ?? throw new NullReferenceException(string.Format("Sorry, There is no such field or property = {0} in class type {1}", new object[2]
            {
                fieldName,
                target.GetType().FullName
            }));
        return this;
    }

    public object GetValue()
    {
        Type type = currentMemberInfo.GetType();
        TypeInfo typeInfo = type.GetTypeInfo();
        return typeInfo.IsSubclassOf(typeof(PropertyInfo))
            ? ((PropertyInfo)currentMemberInfo).GetValue(target, null)
            : typeInfo.IsSubclassOf(typeof(FieldInfo))
            ? ((FieldInfo)currentMemberInfo).GetValue(target)
            : throw new NotSupportedException($"Unsupported type of modified memeber. {type.Name}");
    }

    public ReflectionHelper Property(string propertyName)
    {
        currentMemberInfo = FindProperty(target.GetType(), propertyName);
        return currentMemberInfo is null
            ? throw new NullReferenceException(string.Format("Sorry, There is no such property = {0} in class type {1}", new object[2]
            {
                propertyName,
                target.GetType().FullName
            }))
            : this;
    }

    public T Return<T>()
    {
        return (T)target;
    }

    public ReflectionHelper SetValue(object value)
    {
        if (currentMemberInfo is null)
        {
            throw new NullReferenceException("Modified member is null.");
        }
        Type type = currentMemberInfo.GetType();
        TypeInfo typeInfo = type.GetTypeInfo();
        if (typeInfo.IsSubclassOf(typeof(PropertyInfo)))
        {
            PropertyInfo propertyInfo = (PropertyInfo)currentMemberInfo;
            value = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture.NumberFormat);
            propertyInfo.SetValue(target, value, null);
        }
        else
        {
            if (!typeInfo.IsSubclassOf(typeof(FieldInfo)))
            {
                throw new NotSupportedException($"Unsupported type of modified memeber. {type.Name}");
            }
            FieldInfo fieldInfo = (FieldInfo)currentMemberInfo;
            value = Convert.ChangeType(value, fieldInfo.FieldType, CultureInfo.InvariantCulture.NumberFormat);
            fieldInfo.SetValue(target, value);
        }
        return this;
    }
}
