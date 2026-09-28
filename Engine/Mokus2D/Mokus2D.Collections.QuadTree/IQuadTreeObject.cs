using System;
using Mokus2D.Data;

namespace Mokus2D.Collections.QuadTree;

public interface IQuadTreeObject<T> where T : class, IQuadTreeObject<T>
{
	RectangleFloat Bounds { get; }

	QuadTreeNode<T> Node { get; set; }

	event Action<T> BoundsChanged;
}
