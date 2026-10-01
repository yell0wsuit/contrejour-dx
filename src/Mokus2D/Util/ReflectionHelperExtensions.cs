using System;
using System.Reflection;

namespace Mokus2D.Util
{
    public static class ReflectionHelperExtensions
    {
        public static bool IsInstanceOfType(this Type type, object instance)
        {
            return type.GetTypeInfo().IsAssignableFrom(instance.GetType().GetTypeInfo());
        }
    }
}
