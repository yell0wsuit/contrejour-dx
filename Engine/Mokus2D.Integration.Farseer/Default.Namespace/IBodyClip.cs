using FarseerPhysics.Dynamics;
using Mokus2D.Visual;

namespace Default.Namespace;

public interface IBodyClip
{
	Node Clip { get; }

	Body Body { get; }

	void UpdatePosition();
}
