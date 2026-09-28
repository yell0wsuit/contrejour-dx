using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Mokus2D.Util.Xml;

public class XmlSerializer : XmlSerializerBase
{
    private Assembly _assembly;

    public XmlSerializer(Assembly assembly = null)
    {
        _assembly = assembly;
    }

    public override object DeserializeFile(Stream stream)
    {
        using StreamReader textReader = new StreamReader(stream);
        XDocument xDocument = XDocument.Load(textReader);
        return Deserialize(xDocument.Root);
    }

    public object Deserialize(XElement element)
    {
        string value = element.Attribute("__type__").Value;
        value = UnprocessValue(value);
        if (value == "null")
        {
            return null;
        }
        Type type = null;
        if ((object)_assembly != null)
        {
            type = _assembly.GetType(value);
        }
        if ((object)type == null)
        {
            type = Type.GetType(value);
        }
        object obj = Activator.CreateInstance(type);
        foreach (XAttribute item in element.Attributes())
        {
            if (item.Name != "__type__")
            {
                SetObjectValue(obj, item.Value, item.Name.ToString());
            }
        }
        foreach (XElement item2 in element.Nodes())
        {
            SetObjectValue(obj, Deserialize(item2), item2.Name.ToString());
        }
        return obj;
    }
}
