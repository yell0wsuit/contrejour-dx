using System.Collections.Generic;

using FarseerPhysics.Dynamics;

namespace Mokus2D.Integration.Farseer.Construction;

public interface IPhysicsConfigProcessor
{
    void ProcessConfig(IDictionary<string, string> config, Body body);
}
