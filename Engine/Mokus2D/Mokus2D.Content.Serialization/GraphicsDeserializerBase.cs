using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Data;

namespace Mokus2D.Content.Serialization;

public abstract class GraphicsDeserializerBase<T> : IGraphicsDeserializer<T>, IGraphicsDeserializer
{
	private const string NullString = "null";

	private const string Anchor = "anchor";

	private const string Rect = "rect";

	private IGraphicsLoader loader;

	public abstract bool UseSuffix { get; }

	protected GraphicsDeserializerBase(IGraphicsLoader loader)
	{
		this.loader = loader;
	}

	public abstract T Deserialize(string id, XElement element);

	protected void AddConfig(ConfigData configData, XElement element)
	{
		configData.Config = GetConfig(element);
	}

	protected Dictionary<string, string> GetConfig(XElement element)
	{
		XElement xElement = element.Element("config");
		if (xElement != null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			{
				foreach (XAttribute item in xElement.Attributes())
				{
					dictionary[item.Name.ToString()] = item.Value;
				}
				return dictionary;
			}
		}
		return null;
	}

	protected void SetTexture(XElement document, TextureNodeData result)
	{
		string assetName = (result.TextureName = Path.Combine(new string[2]
		{
			loader.GraphicsRootDirectory,
			(string?)document.Attribute("texture")
		}));
		result.Texture = Mokus2DGame.ContentManager.Load<Texture2D>(assetName);
	}

	protected void SetScaleFactor(TextureNodeData data, XElement element)
	{
		data.ScaleFactor = AttributToFloat(element.Attribute("scaleFactor"), 1f);
	}

	protected Rectangle RectFromXml(XElement element)
	{
		return RectFromString((string?)element.Attribute("rect"));
	}

	protected Vector2 AnchorFromXml(XElement frameXML)
	{
		return VectorFromString((string?)frameXML.Attribute("anchor"));
	}

	protected Rectangle RectFromString(string attribute)
	{
		if (attribute == "null")
		{
			return Rectangle.Empty;
		}
		string[] array = attribute.Split(new char[1] { ' ' });
		Vector2 vector = VectorFromString(array[0]);
		string[] array2 = array[1].Split(new char[1] { 'x' });
		return new Rectangle((int)vector.X, (int)vector.Y, Convert.ToInt32(array2[0]), Convert.ToInt32(array2[1]));
	}

	protected bool AttributeToBool(XAttribute attribute, bool defaultValue)
	{
		if (attribute != null)
		{
			return ToBool((string?)attribute);
		}
		return defaultValue;
	}

	protected int AttributToInt(XAttribute attribute, int defaultValue)
	{
		if (attribute != null)
		{
			return ToInt((string?)attribute);
		}
		return defaultValue;
	}

	protected float AttributToFloat(XAttribute attribute, float defaultValue)
	{
		if (attribute != null)
		{
			return ToSingle((string?)attribute);
		}
		return defaultValue;
	}

	protected Vector2 VectorFromString(string attribute, Vector2 defaultValue)
	{
		if (attribute != null)
		{
			return VectorFromString(attribute);
		}
		return defaultValue;
	}

	protected Vector2 VectorFromString(string attribute)
	{
		if (attribute == "null")
		{
			return Vector2.Zero;
		}
		string[] array = attribute.Split(new char[1] { ',' });
		return new Vector2(ToSingle(array[0]), ToSingle(array[1]));
	}

	protected float ToSingle(string value)
	{
		return Convert.ToSingle(value, CultureInfo.InvariantCulture.NumberFormat);
	}

	protected int ToInt(string value)
	{
		if (value[0] == '#')
		{
			value = value.Substring(1);
			return int.Parse(value, NumberStyles.HexNumber);
		}
		return Convert.ToInt32(value, CultureInfo.InvariantCulture.NumberFormat);
	}

	protected bool ToBool(string value)
	{
		return Convert.ToBoolean(value, CultureInfo.InvariantCulture.NumberFormat);
	}

	object IGraphicsDeserializer.Deserialize(string id, XElement element)
	{
		return Deserialize(id, element);
	}
}
