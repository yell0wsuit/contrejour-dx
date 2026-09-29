using System;
using System.Collections.Generic;

using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening.Repeating;

public class Repeat : ITween, ICleanable, IUpdatable
{
    private static readonly Pool<Repeat> Pool = new(() => new Repeat());

    private ITween _tween;

    private int _times;

    private int _timesExecuted;

    private readonly Queue<Action> _onComplete = new();

    public bool Finished => _timesExecuted >= _times;

    public static Repeat New(ITween tween, int times)
    {
        return Pool.New().Initialize(tween, times);
    }

    private Repeat()
    {
    }

    private Repeat Initialize(ITween tween, int times)
    {
        _tween = tween;
        _times = times;
        return this;
    }

    public void Clean()
    {
        _tween.Clean();
        _times = 0;
        _tween = null;
        _onComplete.Clear();
    }

    public void Free()
    {
        Pool.Free(this);
    }

    public void Reset()
    {
        _timesExecuted = 0;
    }

    public ITween OnComplete(Action action)
    {
        _onComplete.Enqueue(action);
        return this;
    }

    public void Update(float time)
    {
        _tween.Update(time);
        if (!_tween.Finished)
        {
            return;
        }
        _timesExecuted++;
        _tween.Reset();
        if (Finished)
        {
            while (!_onComplete.Empty())
            {
                Action action = _onComplete.Dequeue();
                action();
            }
        }
    }
}
