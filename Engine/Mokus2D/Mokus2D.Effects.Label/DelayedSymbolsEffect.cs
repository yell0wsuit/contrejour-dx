using System;
using Mokus2D.Effects.Tweening;
using Mokus2D.Util;
using Mokus2D.Visual.Text;
using Mokus2D.Visual.Text.LabelData;

namespace Mokus2D.Effects.Label;

public abstract class DelayedSymbolsEffect
{
	public float SymbolDelayTime;

	private readonly Action _onComplete;

	public int Tag;

	public event Action EndEvent;

	protected DelayedSymbolsEffect(float symbolDelayTime)
	{
		SymbolDelayTime = symbolDelayTime;
		Action onComplete = delegate
		{
			this.EndEvent.Dispatch();
		};
		_onComplete = onComplete;
	}

	protected abstract ICompletableTween PlayGlyphEffect(Mokus2D.Visual.Text.Label label, int tag, Glyph glyph, float delay);

	protected float GetGlyphDelay(Mokus2D.Visual.Text.Label label)
	{
		return SymbolDelayTime;
	}

	public void Stop(Mokus2D.Visual.Text.Label label)
	{
		label.Tweener.Stop(Tag);
	}

	public void Play(Mokus2D.Visual.Text.Label label)
	{
		label.RefreshText();
		label.RefreshSymbolsTransform();
		float glyphDelay = GetGlyphDelay(label);
		for (int i = 0; i < label.Glyphs.Count; i++)
		{
			Glyph glyph = label.Glyphs[i];
			ICompletableTween completableTween = PlayGlyphEffect(label, Tag, glyph, glyphDelay * (float)i);
			if (i == label.Glyphs.Count - 1)
			{
				completableTween.OnComplete(_onComplete);
			}
		}
	}
}
