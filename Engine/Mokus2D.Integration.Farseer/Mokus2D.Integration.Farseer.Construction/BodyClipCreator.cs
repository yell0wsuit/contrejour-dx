using System;
using System.Collections.Generic;
using System.Reflection;
using FarseerPhysics.Dynamics;
using Mokus2D.Integration.Farseer.Physics;
using Mokus2D.Visual;

namespace Mokus2D.Integration.Farseer.Construction;

public class BodyClipCreator : IPhysicsConfigProcessor
{
	private readonly string _bodyClipsNamespace;

	private readonly Assembly _bodyClipsAssembly;

	private readonly PhysicsUpdater _updater;

	private readonly ClipFactory _clipFactory;

	public BodyClipCreator(PhysicsUpdater updater, Assembly bodyClipsAssembly, string bodyClipsNamespace, ClipFactory clipFactory)
	{
		_updater = updater;
		_bodyClipsAssembly = bodyClipsAssembly;
		_bodyClipsNamespace = bodyClipsNamespace;
		_clipFactory = clipFactory;
	}

	public void ProcessConfig(IDictionary<string, string> config, Body body)
	{
		string text = config.GetString("bodyClip");
		if (text != null)
		{
			string name = (string.IsNullOrEmpty(_bodyClipsNamespace) ? text : (_bodyClipsNamespace + "." + text));
			Type type = _bodyClipsAssembly.GetType(name);
			Node node = _clipFactory.CreateClip(config, body);
			Activator.CreateInstance(type, _updater, body, node, config);
		}
	}
}
