using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Mokus2D.Util.Xml
{
    public class XmlSerializer(Assembly assembly = null) : XmlSerializerBase
    {
        private readonly Assembly _assembly = assembly;

        public override object DeserializeFile(Stream stream)
        {
            using StreamReader textReader = new(stream);
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
            if (_assembly is not null)
            {
                type = _assembly.GetType(value);
            }
            type ??= Type.GetType(value);
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
