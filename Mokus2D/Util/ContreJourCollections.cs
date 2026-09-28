using System;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Xna.Framework;

namespace Mokus2D.Util;

public static class ContreJourCollections
{
    public static List<Vector2> ToVectorList(this List<object> source)
    {
        List<Vector2> list = new List<Vector2>();
        foreach (Vector2 item in source)
        {
            list.Add(item);
        }
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
                if (checkForNull)
                {
                    throw new Exception("Hashtable key `" + key + "` not found - at `" + text + "`.");
                }
                return null;
            }
            object obj = dictionary[text];
            if (obj == null)
            {
                if (checkForNull)
                {
                    throw new Exception("Hashtable key `" + key + "` is null - at `" + text + "`.");
                }
                return null;
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
        if (source.Exists(key))
        {
            return Convert.ToBoolean(source.GetObject(key));
        }
        return false;
    }

    public static int GetInt(this Dictionary<object, object> source, string key)
    {
        return Convert.ToInt32(source.GetString(key));
    }

    public static uint GetUInt(this Dictionary<object, object> source, string key)
    {
        return Convert.ToUInt32(source.GetString(key));
    }

    public static int GetShort(this Dictionary<object, object> source, string key)
    {
        return Convert.ToInt16(source.GetString(key));
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
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetString(key);
    }

    public static float GetFloat(this Dictionary<object, object> source, string key, float defaultValue)
    {
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetFloat(key);
    }

    public static bool GetBool(this Dictionary<object, object> source, string key, bool defaultValue)
    {
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetBool(key);
    }

    public static int GetInt(this Dictionary<object, object> source, string key, int defaultValue)
    {
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetInt(key);
    }

    public static List<object> GetArrayList(this Dictionary<object, object> source, string key, List<object> defaultValue)
    {
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetArrayList(key);
    }

    public static Vector2 GetVector(this Dictionary<object, object> source, string key, Vector2 defaultValue)
    {
        if (!source.Exists(key))
        {
            return defaultValue;
        }
        return source.GetVector(key);
    }
}
