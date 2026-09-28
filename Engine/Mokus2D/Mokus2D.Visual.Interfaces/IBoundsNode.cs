using Mokus2D.Data;

namespace Mokus2D.Visual.Interfaces;

public interface IBoundsNode : ISizeNode
{
	RectangleFloat Bounds { get; }
}
