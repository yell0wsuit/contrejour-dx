using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace Mokus2D.Localization
{
    public class LocalizationBundle
    {
        private readonly string _name;

        private readonly string _locale;

        private readonly Dictionary<string, string> _messages = [];

        public static string CurrentLocale => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        public LocalizationBundle(string name, string locale = null)
        {
            _name = name;
            _locale = locale;
            Load();
        }

        public string GetLocalizedMessage(string value)
        {
            return _messages.GetValueOrDefault(value);
        }

        private void Load()
        {
            string locale = _locale ?? CurrentLocale;
            Stream stream;
            try
            {
                stream = GetStream(locale);
            }
            catch (Exception)
            {
                stream = GetStream(string.Empty);
            }
            XDocument xDocument = XDocument.Load(stream);
            foreach (XElement item in xDocument.Root.Elements())
            {
                _messages[item.Attribute("name").Value] = item.Value;
            }
        }

        private Stream GetStream(string locale)
        {
            string text = _name;
            if (!string.IsNullOrEmpty(locale))
            {
                text = text + "." + locale;
            }
            string path = $"Resources/{text}.xml";
            return Mokus2DGame.FileLoader.OpenFile(path);
        }
    }
}
