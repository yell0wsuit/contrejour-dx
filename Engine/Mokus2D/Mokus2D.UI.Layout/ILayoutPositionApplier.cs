using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public interface ILayoutPositionApplier
{
	void ApplyPosition(Node node, Vector2 position);
}
