using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Microsoft.Xna.Framework;

namespace ContreJour.Utils
{
    public static class ContreJourCollections
    {
        public static List<Vector2> ToVectorList(this List<object> source)
        {
            List<Vector2> list = [.. source.Cast<Vector2>()];
            return list;
        }

        private static object GetObject(this Dictionary<object, object> source, string key, bool checkForNull)
        {
            string[] array = key.Split('/');
            Dictionary<object, object> dictionary = source;
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i];
                if (!dictionary.ContainsKey(text))
                {
                    return checkForNull ? throw new KeyNotFoundException("Hashtable key `" + key + "` not found - at `" + text + "`.") : null;
                }
                object obj = dictionary[text];
                if (obj == null)
                {
                    return checkForNull ? throw new InvalidOperationException("Hashtable key `" + key + "` is null - at `" + text + "`.") : null;
                }
                if (i == array.Length - 1)
                {
                    return obj;
                }
                dictionary = obj as Dictionary<object, object>;
            }
            return null;
        }

        public static object GetObject(this Dictionary<object, object> source, string key)
        {
            return source.GetObject(key, checkForNull: true);
        }

        public static bool Exists(this Dictionary<object, object> source, string key)
        {
            return source.GetObject(key, checkForNull: false) != null;
        }

        public static bool NotExists(this Dictionary<object, object> source, string key)
        {
            return !source.Exists(key);
        }

        public static Dictionary<object, object> GetHashtable(this Dictionary<object, object> source, string key)
        {
            return source.GetObject(key) as Dictionary<object, object>;
        }

        public static string GetString(this Dictionary<object, object> source, string key)
        {
            return source.GetObject(key) as string;
        }

        public static float GetFloat(this Dictionary<object, object> source, string key)
        {
            return (float)Convert.ToDouble(source.GetString(key), CultureInfo.InvariantCulture);
        }

        public static bool GetBool(this Dictionary<object, object> source, string key)
        {
            return source.Exists(key) && Convert.ToBoolean(source.GetObject(key), CultureInfo.InvariantCulture);
        }

        public static int GetInt(this Dictionary<object, object> source, string key)
        {
            return Convert.ToInt32(source.GetString(key), CultureInfo.InvariantCulture);
        }

        public static uint GetUInt(this Dictionary<object, object> source, string key)
        {
            return Convert.ToUInt32(source.GetString(key), CultureInfo.InvariantCulture);
        }

        public static int GetShort(this Dictionary<object, object> source, string key)
        {
            return Convert.ToInt16(source.GetString(key), CultureInfo.InvariantCulture);
        }

        public static List<object> GetArrayList(this Dictionary<object, object> source, string key)
        {
            return source.GetObject(key) as List<object>;
        }

        public static Vector2 GetVector(this Dictionary<object, object> source, string key)
        {
            return (Vector2)source.GetObject(key);
        }

        public static string GetString(this Dictionary<object, object> source, string key, string defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetString(key);
        }

        public static float GetFloat(this Dictionary<object, object> source, string key, float defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetFloat(key);
        }

        public static bool GetBool(this Dictionary<object, object> source, string key, bool defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetBool(key);
        }

        public static int GetInt(this Dictionary<object, object> source, string key, int defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetInt(key);
        }

        public static List<object> GetArrayList(this Dictionary<object, object> source, string key, List<object> defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetArrayList(key);
        }

        public static Vector2 GetVector(this Dictionary<object, object> source, string key, Vector2 defaultValue)
        {
            return !source.Exists(key) ? defaultValue : source.GetVector(key);
        }
    }
}
