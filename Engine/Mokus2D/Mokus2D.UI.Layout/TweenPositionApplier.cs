using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public class TweenPositionApplier : ILayoutPositionApplier
{
    private readonly float _effectTime;

    private readonly int? _tag;

    public TweenPositionApplier(float effectTime, int? tag = null)
    {
        _effectTime = effectTime;
        _tag = tag;
    }

    public void ApplyPosition(Node node, Vector2 position)
    {
        int? tag = _tag;
        if (tag.HasValue)
        {
            Tweener tweener = node.Tweener;
            int? tag2 = _tag;
            tweener.Stop(tag2.Value);
        }
        node.Tweener.Start(_effectTime, _tag).Tween(NodeValues.Position, position);
    }
}
