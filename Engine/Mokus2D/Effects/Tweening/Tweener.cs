using System;
using System.Collections.Generic;

using Mokus2D.Collections;
using Mokus2D.Data;
using Mokus2D.Effects.Tweening.Repeating;
using Mokus2D.Interfaces;
using Mokus2D.Util.Resources;

namespace Mokus2D.Effects.Tweening;

public class Tweener(object defaultTarget) : DisposableBase, IUpdatable, ICleanable
{
    private readonly struct TweenAndTag(ITween tween, int? tag)
    {
        public readonly ITween Tween = tween;

        public readonly int? Tag = tag;
    }

    private readonly object _defaultTarget = defaultTarget;

    private readonly ForEachCollection<TweenAndTag> _tweens = [];

    public TweenObject RepeatForever(float seconds, int? tag = null, object target = null)
    {
        TweenObject tweenObject = Create(seconds, target);
        RepeatForever tween = Repeating.RepeatForever.New(tweenObject);
        Start(tween, tag);
        return tweenObject;
    }

    public Sequence RepeatSequenceForever(float seconds, int? tag = null, object target = null)
    {
        Sequence sequence = CreateSequence(seconds, target);
        RepeatForever tween = Repeating.RepeatForever.New(sequence);
        Start(tween, tag);
        return sequence;
    }

    public void Start(ITween tween, int? tag = null)
    {
        lock (_tweens)
        {
            _tweens.Add(new TweenAndTag(tween, tag));
        }
    }

    public Sequence StartSequence()
    {
        Sequence sequence = CreateSequence();
        Start(sequence);
        return sequence;
    }

    private Sequence CreateSequence()
    {
        return Sequence.New(_defaultTarget);
    }

    public Sequence StartSequence(ITween tween, int? tag = null)
    {
        Sequence sequence = CreateSequence(tween);
        lock (_tweens)
        {
            _tweens.Add(new TweenAndTag(sequence, tag));
            return sequence;
        }
    }

    public Sequence CreateSequence(ITween tween)
    {
        Sequence sequence = Sequence.New(_defaultTarget);
        _ = sequence.Next(tween);
        return sequence;
    }

    public TweenObject Start(float seconds, object target)
    {
        return Start(seconds, null, target);
    }

    public TweenObject Start(float seconds, int? tag = null, object target = null)
    {
        TweenObject tweenObject = Create(seconds, target);
        lock (_tweens)
        {
            _tweens.Add(new TweenAndTag(tweenObject, tag));
            return tweenObject;
        }
    }

    public TweenObject Create(float seconds, object target = null)
    {
        return TweenObject.New(target ?? _defaultTarget, seconds);
    }

    public Sequence StartSequence(float seconds, object target = null)
    {
        return StartSequence(seconds, null, target);
    }

    public Sequence StartSequence(float seconds, int tag, object target = null)
    {
        return StartSequence(seconds, (int?)tag, target);
    }

    private Sequence StartSequence(float seconds, int? tag, object target)
    {
        Sequence sequence = CreateSequence(seconds, target);
        lock (_tweens)
        {
            _tweens.Add(new TweenAndTag(sequence, tag));
            return sequence;
        }
    }

    public Sequence CreateSequence(float seconds, object target = null)
    {
        return Sequence.New(target ?? _defaultTarget).Next(seconds, target);
    }

    public void Update(float time)
    {
        lock (_tweens)
        {
            using (_tweens.Using())
            {
                foreach (TweenAndTag tween in _tweens)
                {
                    tween.Tween.Update(time);
                    if (tween.Tween.Finished)
                    {
                        tween.Tween.Free();
                        _ = _tweens.Remove(tween);
                    }
                }
            }
        }
    }

    public void Stop()
    {
        Clean();
    }

    public void Clean()
    {
        lock (_tweens)
        {
            if (_tweens.Empty())
            {
                return;
            }
            foreach (TweenAndTag tween in _tweens)
            {
                tween.Tween.Free();
            }
            _tweens.Clear();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Stop();
    }

    public void Stop(int? tag)
    {
        if (!tag.HasValue)
        {
            Stop();
        }
        else
        {
            Stop(tag.Value);
        }
    }

    public void Stop(int tag)
    {
        for (int num = _tweens.Count - 1; num >= 0; num--)
        {
            if (_tweens[num].Tag == tag)
            {
                _tweens.RemoveAt(num);
            }
        }
    }

    public void StopTweenObjects()
    {
        Stop(TweenerPredicates.TweenObjectPredicate);
    }

    public void StopSequences()
    {
        Stop(TweenerPredicates.SequencePredicate);
    }

    public void StopObjectSequences(object o)
    {
        Stop(t => t is Sequence sequence && sequence.Target == o);
    }

    public void Stop(Predicate<ITween> predicate)
    {
        for (int num = _tweens.Count - 1; num >= 0; num--)
        {
            if (predicate(_tweens[num].Tween))
            {
                _tweens.RemoveAt(num);
            }
        }
    }
}
