using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class Hashtable : Dictionary<object, object>
{
    private object GetObject(string key, bool checkForNull)
    {
        string[] array = key.Split('/');
        Hashtable hashtable = this;
        for (int i = 0; i < array.Length; i++)
        {
            string text = array[i];
            if (!hashtable.ContainsKey(text))
            {
                return checkForNull ? throw new KeyNotFoundException("Hashtable key `" + key + "` not found - at `" + text + "`.") : null;
            }
            object obj = hashtable[text];
            if (obj == null)
            {
                return checkForNull ? throw new InvalidOperationException("Hashtable key `" + key + "` is null - at `" + text + "`.") : null;
            }
            if (i == array.Length - 1)
            {
                return obj;
            }
            hashtable = obj as Hashtable;
        }
        return null;
    }

    public object GetObject(string key)
    {
        return GetObject(key, checkForNull: true);
    }

    public bool Exists(string key)
    {
        return GetObject(key, checkForNull: false) != null;
    }

    public bool NotExists(string key)
    {
        return !Exists(key);
    }

    public Hashtable GetHashtable(string key)
    {
        return GetObject(key) as Hashtable;
    }

    public string GetString(string key)
    {
        return GetObject(key) as string;
    }

    public float GetFloat(string key)
    {
        return (float)Convert.ToDouble(GetString(key), CultureInfo.InvariantCulture);
    }

    public bool GetBool(string key)
    {
        return Exists(key) && Convert.ToBoolean(GetObject(key), CultureInfo.InvariantCulture);
    }

    public int GetInt(string key)
    {
        return Convert.ToInt32(GetString(key), CultureInfo.InvariantCulture);
    }

    public uint GetUInt(string key)
    {
        return Convert.ToUInt32(GetString(key), CultureInfo.InvariantCulture);
    }

    public int GetShort(string key)
    {
        return Convert.ToInt16(GetString(key), CultureInfo.InvariantCulture);
    }

    public List<object> GetArrayList(string key)
    {
        return GetObject(key) as List<object>;
    }

    public Vector2 GetVector(string key)
    {
        return (Vector2)GetObject(key);
    }

    public string GetString(string key, string defaultValue)
    {
        return !Exists(key) ? defaultValue : GetString(key);
    }

    public float GetFloat(string key, float defaultValue)
    {
        return !Exists(key) ? defaultValue : GetFloat(key);
    }

    public bool GetBool(string key, bool defaultValue)
    {
        return !Exists(key) ? defaultValue : GetBool(key);
    }

    public int GetInt(string key, int defaultValue)
    {
        return !Exists(key) ? defaultValue : GetInt(key);
    }

    public List<object> GetArrayList(string key, List<object> defaultValue)
    {
        return !Exists(key) ? defaultValue : GetArrayList(key);
    }

    public Vector2 GetVector(string key, Vector2 defaultValue)
    {
        return !Exists(key) ? defaultValue : GetVector(key);
    }

    public override string ToString()
    {
        using StringWriter stringWriter = new();
        using (Enumerator enumerator = GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                KeyValuePair<object, object> current = enumerator.Current;
                string text = "<null>";
                if (current.Value != null)
                {
                    text = current.Value.ToString();
                    if (current.Value is Hashtable)
                    {
                        StringWriter stringWriter2 = new();
                        using (StringReader stringReader = new(text))
                        {
                            string text2;
                            while ((text2 = stringReader.ReadLine()) != null)
                            {
                                stringWriter2.WriteLine("|    " + text2);
                            }
                        }
                        text = "\n" + stringWriter2;
                    }
                }
                stringWriter.WriteLine(string.Concat(current.Key, ": ", text));
            }
        }
        return stringWriter.ToString();
    }

    public static void Trace()
    {
    }
}
