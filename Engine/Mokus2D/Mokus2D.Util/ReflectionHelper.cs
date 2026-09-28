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
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		this.target = target;
	}

	public static FieldInfo FindField(Type type, string name)
	{
		if ((object)type == null || (object)type == typeof(object))
		{
			return null;
		}
		TypeInfo typeInfo = type.GetTypeInfo();
		return typeInfo.GetDeclaredField(name) ?? FindField(typeInfo.BaseType, name);
	}

	public static PropertyInfo FindProperty(Type type, string name)
	{
		if ((object)type == null || (object)type == typeof(object))
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
		if ((object)currentMemberInfo == null)
		{
			throw new NullReferenceException(string.Format("Sorry, There is no such field = {0} in class type {1}", new object[2]
			{
				fieldName,
				target.GetType().FullName
			}));
		}
		return this;
	}

	public ReflectionHelper FieldOrProperty(string fieldName)
	{
		currentMemberInfo = FindField(target.GetType(), fieldName);
		if ((object)currentMemberInfo == null)
		{
			currentMemberInfo = FindProperty(target.GetType(), fieldName);
			if ((object)currentMemberInfo == null)
			{
				throw new NullReferenceException(string.Format("Sorry, There is no such field or property = {0} in class type {1}", new object[2]
				{
					fieldName,
					target.GetType().FullName
				}));
			}
		}
		return this;
	}

	public object GetValue()
	{
		Type type = currentMemberInfo.GetType();
		TypeInfo typeInfo = type.GetTypeInfo();
		if (typeInfo.IsSubclassOf(typeof(PropertyInfo)))
		{
			return ((PropertyInfo)currentMemberInfo).GetValue(target, null);
		}
		if (typeInfo.IsSubclassOf(typeof(FieldInfo)))
		{
			return ((FieldInfo)currentMemberInfo).GetValue(target);
		}
		throw new NotSupportedException($"Unsupported type of modified memeber. {type.Name}");
	}

	public ReflectionHelper Property(string propertyName)
	{
		currentMemberInfo = FindProperty(target.GetType(), propertyName);
		if ((object)currentMemberInfo == null)
		{
			throw new NullReferenceException(string.Format("Sorry, There is no such property = {0} in class type {1}", new object[2]
			{
				propertyName,
				target.GetType().FullName
			}));
		}
		return this;
	}

	public T Return<T>()
	{
		return (T)target;
	}

	public ReflectionHelper SetValue(object value)
	{
		if ((object)currentMemberInfo == null)
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
