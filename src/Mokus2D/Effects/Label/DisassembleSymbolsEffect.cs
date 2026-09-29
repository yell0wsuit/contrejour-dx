using System.Numerics;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Effects.Tweening;
using Mokus2D.Visual;
using Mokus2D.Visual.Text.LabelData;

namespace Mokus2D.Effects.Label
{
    public class DisassembleSymbolsEffect : AssembleSymbolsEffect
    {
        public DisassembleSymbolsEffect(float symbolDelayTime)
            : base(symbolDelayTime)
        {
        }

        public DisassembleSymbolsEffect(float symbolDelayTime, Vector2 tweenOffset, Vector2 tweenScale, float tweenTime)
            : base(symbolDelayTime, tweenOffset, tweenScale, tweenTime)
        {
        }

        protected override ICompletableTween PlayGlyphEffect(Visual.Text.Label label, int tag, Glyph glyph, float delay)
        {
            return label.Tweener.StartSequence(delay, tag, glyph).Next(TweenTime).Tween(NodeValues.Position, glyph.Position + TweenOffset)
                .Tween(NodeValues.ScaleVec, glyph.ScaleVec * TweenScale)
                .Tween(NodeValues.OpacityFloat, 0f)
                .Ease(Cubic.EaseInOut);
        }
    }
}
