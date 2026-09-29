using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;
using Mokus2D.Visual.Text.LabelData;

namespace Mokus2D.Effects.Label;

public class AssembleSymbolsEffect(float symbolDelayTime, Vector2 tweenOffset, Vector2 tweenScale, float tweenTime) : DelayedSymbolsEffect(symbolDelayTime)
{
    protected Vector2 TweenOffset { get; } = tweenOffset;

    protected Vector2 TweenScale { get; } = tweenScale;

    public float TweenTime { get; set; } = tweenTime;

    public AssembleSymbolsEffect(float symbolDelayTime)
        : this(symbolDelayTime, new Vector2(20f, 0f), new Vector2(3f, 1f), 0.2f)
    {
    }

    protected override ICompletableTween PlayGlyphEffect(Visual.Text.Label label, int tag, Glyph glyph, float delay)
    {
        Vector2 position = glyph.Position;
        Vector2 scaleVec = glyph.ScaleVec;
        glyph.Position += TweenOffset;
        glyph.ScaleVec *= TweenScale;
        glyph.OpacityFloat = 0f;
        return label.Tweener.StartSequence(delay, Tag, glyph).Next(TweenTime).Tween(NodeValues.Position, position)
            .Tween(NodeValues.ScaleVec, scaleVec)
            .Tween(NodeValues.OpacityFloat, 1f)
            .Ease(Cubic.EaseInOut);
    }
}
