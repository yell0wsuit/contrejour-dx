using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public abstract class LineLayout(Node container) : LayoutBase(container)
{
    public float Margins { get; set; }

    private readonly LayoutDirection Direction = LayoutDirection.Normal;

    private readonly DefaultPositionApplier PositionApplier = new DefaultPositionApplier();

    public float? FixedSize;

    public float TotalSize { get; private set; }

    protected virtual IList<Node> LayoutNodes => Container.Children;

    public virtual void Apply()
    {
        Vector2 currentPosition = Vector2.Zero;
        TotalSize = 0f;
        if (LayoutNodes.Empty())
        {
            return;
        }
        for (int i = 0; i < LayoutNodes.Count; i++)
        {
            Node node = LayoutNodes[i];
            if (node.Visible)
            {
                Vector2 position = currentPosition;
                if (node is ISizeNode sizeNode)
                {
                    float num = FixedSize ?? GetNodeSize(sizeNode);
                    float num2 = (num + Margins) * (float)Direction;
                    ChangePosition(ref currentPosition, num2);
                    TotalSize += num2;
                }
                PositionApplier.ApplyPosition(node, position);
            }
        }
        TotalSize -= Margins * (float)Direction;
    }

    protected abstract float GetNodeSize(ISizeNode sizeNode);

    protected abstract void ChangePosition(ref Vector2 currentPosition, float change);
}
