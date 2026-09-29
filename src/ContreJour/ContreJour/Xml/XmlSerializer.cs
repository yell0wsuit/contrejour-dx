using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ContreJour.Xml
{
    public class XmlSerializer(Assembly assembly = null) : XmlSerializerBase
    {
        private readonly Assembly _assembly = assembly;

        public override object DeserializeText(string text)
        {
            using StringReader textReader = new(text);
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
            if (_assembly is not null)
            {
                type = _assembly.GetType(text);
            }
            type ??= Type.GetType(text);
            object obj = Activator.CreateInstance(type);
            foreach (XAttribute item in element.Attributes())
            {
                if (item.Name != "__type__")
                {
                    SetObjectValue(obj, item.Value, item.Name.ToString());
                }
            }
            foreach (XElement item2 in element.Nodes().Cast<XElement>())
            {
                SetObjectValue(obj, Deserialize(item2), item2.Name.ToString());
            }
            return obj;
        }
    }
}
