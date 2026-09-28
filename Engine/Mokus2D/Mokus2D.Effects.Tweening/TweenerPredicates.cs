using System;

namespace Mokus2D.Effects.Tweening;

public static class TweenerPredicates
{
	public static readonly Predicate<ITween> SequencePredicate = (ITween tween) => tween is Sequence;

	public static readonly Predicate<ITween> TweenObjectPredicate = (ITween tween) => tween is TweenObject;
}
