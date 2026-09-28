using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction;

public interface IClipFactory
{
	Node CreateClip(IDictionary<string, string> config, Body body);
}
