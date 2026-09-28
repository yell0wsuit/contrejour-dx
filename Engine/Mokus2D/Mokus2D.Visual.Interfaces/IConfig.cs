using System.Collections.Generic;

namespace Mokus2D.Visual.Interfaces;

public interface IConfig
{
	IDictionary<string, string> Config { get; }
}
