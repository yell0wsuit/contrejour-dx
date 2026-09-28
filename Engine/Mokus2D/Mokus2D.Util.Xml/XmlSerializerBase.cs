using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Mokus2D.Util.Xml;

public abstract class XmlSerializerBase
{
    private Dictionary<string, string> aliases = [];

    public void AddAlias(string source, string alias)
    {
        aliases[alias] = source;
    }

    public abstract object DeserializeFile(Stream stream);

    protected string UnprocessValue(string source)
    {
        foreach (KeyValuePair<string, string> alias in aliases)
        {
            if (string.Equals(source, alias.Value, StringComparison.OrdinalIgnoreCase))
            {
                return alias.Key;
            }
        }
        return source;
    }

    protected void SetObjectValue(object target, object targetValue, string key)
    {
        key = UnprocessAttributeName(key);
        if (target is IList)
        {
            int num = Convert.ToInt32(key, CultureInfo.InvariantCulture);
            IList list = target as IList;
            while (list.Count < num + 1)
            {
                _ = list.Add(null);
            }
            list[num] = targetValue;
        }
        else if (target is IDictionary)
        {
            ((IDictionary)target)[key] = targetValue;
        }
        else
        {
            _ = target.Reflect().FieldOrProperty(key).SetValue(targetValue);
        }
    }

    private string UnprocessAttributeName(string attributeName)
    {
        return attributeName.StartsWith("__", StringComparison.Ordinal) ? attributeName[2..] : UnprocessValue(attributeName);
    }
}
