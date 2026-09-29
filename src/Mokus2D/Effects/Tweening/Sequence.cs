using System;
using System.Collections.Generic;
using System.Diagnostics;

using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening
{
    public class Sequence : ITween<Sequence>, ICompletableTween, ITween, ICleanable, IUpdatable
    {
        private static readonly Pool<Sequence> Pool = new(() => new Sequence());

        private readonly List<ITween> _tweens = [];

        private int _currentIndex;

        private bool Test;

        private ITween Current => _tweens[_currentIndex];

        private TweenObject LastTween => (TweenObject)_tweens[^1];

        public bool Finished => _currentIndex >= _tweens.Count;

        public object Target { get; private set; }

        public static Sequence New(object defaultTarget)
        {
            return Pool.New().Initialize(defaultTarget);
        }

        private Sequence()
        {
        }

        private Sequence Initialize(object defaultTarget)
        {
            Target = defaultTarget;
            return this;
        }

        public Sequence Next(float seconds, object target = null)
        {
            TweenObject tween = TweenObject.New(target ?? Target, seconds);
            return Next(tween);
        }

        public Sequence Next(ITween tween)
        {
            _tweens.Add(tween);
            return this;
        }

        public void Clean()
        {
            foreach (ITween tween in _tweens)
            {
                tween.Free();
            }
            if (Test)
            {
                Trace.TraceInformation("huj");
            }
            _tweens.Clear();
            Target = null;
            _currentIndex = 0;
            Test = false;
        }

        public void Update(float time)
        {
            if (Test)
            {
                Trace.TraceInformation("huj");
            }
            Current.Update(time);
            if (Current.Finished)
            {
                _currentIndex++;
            }
        }

        public void Free()
        {
            Pool.Free(this);
        }

        public void Reset()
        {
            _currentIndex = 0;
            foreach (ITween tween in _tweens)
            {
                tween.Reset();
            }
        }

        ICompletableTween ICompletableTween.OnComplete(Action action)
        {
            return OnComplete(action);
        }

        public Sequence Tween<TValue>(GetSetValue<TValue> getSet, TValue targetValue, Func<float, float, float, float> easing, float easingParamA, float easingParamB)
        {
            _ = LastTween.Tween(getSet, targetValue, easing, easingParamA, easingParamB);
            return this;
        }

        public Sequence Tween<T>(GetSetValue<T> getSet, T targetValue, Func<float, float, float> easing, float easingParam)
        {
            _ = LastTween.Tween(getSet, targetValue, easing, easingParam);
            return this;
        }

        public Sequence Tween<T>(GetSetValue<T> getSet, T targetValue, Func<float, float> easing)
        {
            _ = LastTween.Tween(getSet, targetValue, easing);
            return this;
        }

        public Sequence Tween<T>(GetSetValue<T> getSet, T targetValue)
        {
            _ = LastTween.Tween(getSet, targetValue);
            return this;
        }

        public Sequence Set<T>(GetSetValue<T> getSet, T targetValue)
        {
            _ = LastTween.Set(getSet, targetValue);
            return this;
        }

        public Sequence SetAfter<T>(GetSetValue<T> getSet, T targetValue)
        {
            _ = LastTween.SetAfter(getSet, targetValue);
            return this;
        }

        public Sequence Ease(Func<float, float> easing)
        {
            _ = LastTween.Ease(easing);
            return this;
        }

        public Sequence Ease(Func<float, float, float> easing, float easingParam)
        {
            _ = LastTween.Ease(easing, easingParam);
            return this;
        }

        public Sequence Ease(Func<float, float, float, float> easing, float easingParamA, float easingParamB)
        {
            _ = LastTween.Ease(easing, easingParamA, easingParamB);
            return this;
        }

        public Sequence OnComplete(Action<object> action)
        {
            _ = LastTween.OnComplete(action);
            return this;
        }

        public Sequence OnComplete(Action action)
        {
            _ = LastTween.OnComplete(action);
            return this;
        }
    }
}
