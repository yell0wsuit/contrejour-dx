using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Mokus2D.Util.Xml;

public abstract class XmlSerializerBase
{
	private Dictionary<string, string> aliases = new Dictionary<string, string>();

	public void AddAlias(string source, string alias)
	{
		aliases[alias] = source;
	}

	public abstract object DeserializeFile(Stream stream);

	protected string UnprocessValue(string source)
	{
		foreach (KeyValuePair<string, string> alias in aliases)
		{
			if (source.ToLower().Equals(alias.Value.ToLower()))
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
			int num = Convert.ToInt32(key);
			IList list = target as IList;
			while (list.Count < num + 1)
			{
				list.Add(null);
			}
			list[num] = targetValue;
		}
		else if (target is IDictionary)
		{
			((IDictionary)target)[key] = targetValue;
		}
		else
		{
			target.Reflect().FieldOrProperty(key).SetValue(targetValue);
		}
	}

	private string UnprocessAttributeName(string attributeName)
	{
		if (attributeName.StartsWith("__"))
		{
			return attributeName.Substring(2);
		}
		return UnprocessValue(attributeName);
	}
}
