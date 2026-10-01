using System;
using System.Reflection;

namespace Mokus2D.Util
{
    public static class ReflectUtil
    {
        public static object CreateInstance(Type mainAssemblyClass, string typeName, params object[] parameters)
        {
            Type type = GetType(typeName, mainAssemblyClass);
            return Activator.CreateInstance(type, parameters);
        }

        private static Type GetType(string typeName, Type mainAssemblyClass)
        {
            Assembly assembly = mainAssemblyClass.GetTypeInfo().Assembly;
            return assembly.GetType(typeName);
        }

        public static object CreateInstance(Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }

        public static TypeInfo CastToTypeInfo(object obj)
        {
            return (TypeInfo)obj;
        }
    }
}
