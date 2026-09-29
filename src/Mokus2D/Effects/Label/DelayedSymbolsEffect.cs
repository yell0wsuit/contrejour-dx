using System;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util;
using Mokus2D.Visual.Text.LabelData;

namespace Mokus2D.Effects.Label;

public abstract class DelayedSymbolsEffect
{
    private readonly float SymbolDelayTime;

    private readonly Action _onComplete;

    public int Tag { get; set; }

    public event Action EndEvent;

    protected DelayedSymbolsEffect(float symbolDelayTime)
    {
        SymbolDelayTime = symbolDelayTime;
        Action onComplete = EndEvent.Dispatch;
        _onComplete = onComplete;
    }

    protected abstract ICompletableTween PlayGlyphEffect(Visual.Text.Label label, int tag, Glyph glyph, float delay);

    protected float GetGlyphDelay()
    {
        return SymbolDelayTime;
    }

    public void Stop(Visual.Text.Label label)
    {
        label.Tweener.Stop(Tag);
    }

    public void Play(Visual.Text.Label label)
    {
        label.RefreshText();
        label.RefreshSymbolsTransform();
        float glyphDelay = GetGlyphDelay();
        for (int i = 0; i < label.Glyphs.Count; i++)
        {
            Glyph glyph = label.Glyphs[i];
            ICompletableTween completableTween = PlayGlyphEffect(label, Tag, glyph, glyphDelay * i);
            if (i == label.Glyphs.Count - 1)
            {
                _ = completableTween.OnComplete(_onComplete);
            }
        }
    }
}
