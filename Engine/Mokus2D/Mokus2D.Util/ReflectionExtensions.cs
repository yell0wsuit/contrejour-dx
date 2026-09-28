using System.Reflection;

namespace Mokus2D.Util;

public static class ReflectionExtensions
{
	public static FieldInfo GetField(this TypeInfo typeInfo, string name)
	{
		return typeInfo.GetDeclaredField(name);
	}

	public static PropertyInfo GetProperty(this TypeInfo typeInfo, string name)
	{
		return typeInfo.GetDeclaredProperty(name);
	}
}
