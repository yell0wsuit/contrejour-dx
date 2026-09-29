using System;
using System.Collections.Generic;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data
{
    [Serializable]
    public class ConfigData : IConfig
    {
        public IDictionary<string, string> Config { get; set; }
    }
}
