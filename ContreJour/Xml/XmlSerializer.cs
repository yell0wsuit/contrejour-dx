using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace ContreJour.Xml;

public class XmlSerializer : XmlSerializerBase
{
    private readonly Assembly _assembly;

    public XmlSerializer(Assembly assembly = null)
    {
        _assembly = assembly;
    }

    public override object DeserializeText(string text)
    {
        using StringReader textReader = new StringReader(text);
        return Deserialize(XDocument.Load(textReader).Root);
    }

    public object Deserialize(XElement element)
    {
        string text = UnprocessValue(element.Attribute("__type__").Value);
        if (text == "null")
        {
            return null;
        }
        Type type = null;
        if ((object)_assembly != null)
        {
            type = _assembly.GetType(text);
        }
        if ((object)type == null)
        {
            type = Type.GetType(text);
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
