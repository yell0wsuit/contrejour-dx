using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;

namespace Mokus2D.Content.Serialization;

public abstract class GraphicsDeserializerBase<T>(IGraphicsLoader loader) : IGraphicsDeserializer<T>, IGraphicsDeserializer
{
    private readonly IGraphicsLoader loader = loader;

    public abstract bool UseSuffix { get; }

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
            Dictionary<string, string> dictionary = [];
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
        string assetName = result.TextureName = Path.Combine(
        [
            loader.GraphicsRootDirectory,
            (string)document.Attribute("texture")
        ]);
        result.Texture = Mokus2DGame.ContentManager.Load<Texture2D>(assetName);
    }

    protected void SetScaleFactor(TextureNodeData data, XElement element)
    {
        data.ScaleFactor = AttributToFloat(element.Attribute("scaleFactor"), 1f);
    }

    protected Rectangle RectFromXml(XElement element)
    {
        return RectFromString((string)element.Attribute("rect"));
    }

    protected Vector2 AnchorFromXml(XElement frameXML)
    {
        return VectorFromString((string)frameXML.Attribute("anchor"));
    }

    protected Rectangle RectFromString(string attribute)
    {
        if (attribute == "null")
        {
            return Rectangle.Empty;
        }
        string[] array = attribute.Split([' ']);
        Vector2 vector = VectorFromString(array[0]);
        string[] array2 = array[1].Split(['x']);
        return new Rectangle((int)vector.X, (int)vector.Y, Convert.ToInt32(array2[0], CultureInfo.InvariantCulture), Convert.ToInt32(array2[1], CultureInfo.InvariantCulture));
    }

    protected bool AttributeToBool(XAttribute attribute, bool defaultValue)
    {
        return attribute != null ? ToBool((string)attribute) : defaultValue;
    }

    protected int AttributToInt(XAttribute attribute, int defaultValue)
    {
        return attribute != null ? ToInt((string)attribute) : defaultValue;
    }

    protected float AttributToFloat(XAttribute attribute, float defaultValue)
    {
        return attribute != null ? ToSingle((string)attribute) : defaultValue;
    }

    protected Vector2 VectorFromString(string attribute, Vector2 defaultValue)
    {
        return attribute != null ? VectorFromString(attribute) : defaultValue;
    }

    protected Vector2 VectorFromString(string attribute)
    {
        if (attribute == "null")
        {
            return Vector2.Zero;
        }
        string[] array = attribute.Split([',']);
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
            value = value[1..];
            return int.Parse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
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
