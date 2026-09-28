using System;

namespace Mokus2D.Effects.Tweening;

public static class TweenerPredicates
{
    public static readonly Predicate<ITween> SequencePredicate = tween => tween is Sequence;

    public static readonly Predicate<ITween> TweenObjectPredicate = tween => tween is TweenObject;
}
