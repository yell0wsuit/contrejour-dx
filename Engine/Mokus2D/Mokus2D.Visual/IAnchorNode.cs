using Microsoft.Xna.Framework;

namespace Mokus2D.Visual;

public interface IAnchorNode : ISizeNode
{
	Vector2 Anchor { get; set; }
}
