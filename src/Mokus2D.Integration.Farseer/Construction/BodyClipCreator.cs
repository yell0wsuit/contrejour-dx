using System;
using System.Collections.Generic;
using System.Reflection;

using FarseerPhysics.Dynamics;

using Mokus2D.Integration.Farseer.Physics;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction;

public class BodyClipCreator(PhysicsUpdater updater, Assembly bodyClipsAssembly, string bodyClipsNamespace, ClipFactory clipFactory) : IPhysicsConfigProcessor
{
    private readonly string _bodyClipsNamespace = bodyClipsNamespace;

    private readonly Assembly _bodyClipsAssembly = bodyClipsAssembly;

    private readonly PhysicsUpdater _updater = updater;

    private readonly ClipFactory _clipFactory = clipFactory;

    public void ProcessConfig(IDictionary<string, string> config, Body body)
    {
        string text = config.GetString("bodyClip");
        if (text != null)
        {
            string name = string.IsNullOrEmpty(_bodyClipsNamespace) ? text : (_bodyClipsNamespace + "." + text);
            Type type = _bodyClipsAssembly.GetType(name);
            Node node = _clipFactory.CreateClip(config, body);
            _ = Activator.CreateInstance(type, _updater, body, node, config);
        }
    }
}
