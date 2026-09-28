using Microsoft.Xna.Framework;
using Mokus2D.Interfaces;

namespace Mokus2D.Visual.Displacement.Magnets;

public interface IGridMagnet : IUpdatable
{
	bool HasRemove { get; }

	Rectangle Bounds { get; }

	Vector2 Position { get; }

	Vector2 GetForce(Vector2 relativePosition);
}
