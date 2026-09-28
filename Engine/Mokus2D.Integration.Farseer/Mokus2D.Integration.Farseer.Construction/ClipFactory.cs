using System;
using System.Collections.Generic;
using System.Reflection;

using FarseerPhysics.Dynamics;

using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction;

public class ClipFactory : IClipFactory
{
    private readonly string _namespace;

    private readonly Assembly _clipsAssembly;

    private readonly Node _clipsNode;

    public ClipFactory(Node clipsNode, Assembly clipsAssembly, string @namespace)
    {
        _clipsAssembly = clipsAssembly;
        _namespace = @namespace;
        _clipsNode = clipsNode;
    }

    public Node CreateClip(IDictionary<string, string> config, Body body)
    {
        string text = config.GetString("clip");
        if (text != null)
        {
            string name = (string.IsNullOrEmpty(_namespace) ? text : (_namespace + "." + text));
            Type type = _clipsAssembly.GetType(name);
            Node node = (Node)Activator.CreateInstance(type);
            _clipsNode.AddChild(node);
            return node;
        }
        return null;
    }
}
