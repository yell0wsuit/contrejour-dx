using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            if (!hashtable.Keys.Contains(text))
            {
                if (checkForNull)
                {
                    throw new Exception("Hashtable key `" + key + "` not found - at `" + text + "`.");
                }
                return null;
            }
            object obj = hashtable[text];
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
        if (Exists(key))
        {
            return Convert.ToBoolean(GetObject(key));
        }
        return false;
    }

    public int GetInt(string key)
    {
        return Convert.ToInt32(GetString(key));
    }

    public uint GetUInt(string key)
    {
        return Convert.ToUInt32(GetString(key));
    }

    public int GetShort(string key)
    {
        return Convert.ToInt16(GetString(key));
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
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetString(key);
    }

    public float GetFloat(string key, float defaultValue)
    {
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetFloat(key);
    }

    public bool GetBool(string key, bool defaultValue)
    {
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetBool(key);
    }

    public int GetInt(string key, int defaultValue)
    {
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetInt(key);
    }

    public List<object> GetArrayList(string key, List<object> defaultValue)
    {
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetArrayList(key);
    }

    public Vector2 GetVector(string key, Vector2 defaultValue)
    {
        if (!Exists(key))
        {
            return defaultValue;
        }
        return GetVector(key);
    }

    public override string ToString()
    {
        using StringWriter stringWriter = new StringWriter();
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
                        StringWriter stringWriter2 = new StringWriter();
                        using (StringReader stringReader = new StringReader(text))
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

    public void Trace()
    {
    }
}
