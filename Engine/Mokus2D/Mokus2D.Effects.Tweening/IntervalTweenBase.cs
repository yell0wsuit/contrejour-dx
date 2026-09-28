using System;
using System.Collections.Generic;

using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening;

public abstract class IntervalTweenBase : ICompletableTween, ITween, ICleanable, IUpdatable
{
    private float _seconds;

    private float _elapsed;

    private readonly Queue<Action> _onComplete = new(64);

    private bool _started;

    public bool Finished { get; private set; }

    protected abstract void UpdateRatio(float ratio);

    public abstract void Free();

    protected abstract void Start();

    protected virtual void Finish()
    {
        while (!_onComplete.Empty())
        {
            Action action = _onComplete.Dequeue();
            action();
        }
    }

    protected void Initialize(float seconds)
    {
        _seconds = seconds;
        _elapsed = 0f;
        Finished = false;
    }

    ICompletableTween ICompletableTween.OnComplete(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _onComplete.Enqueue(action);
        return this;
    }

    public void Update(float time)
    {
        if (!_started)
        {
            _started = true;
            Start();
        }
        _elapsed += time;
        float num = _elapsed / _seconds;
        UpdateRatio(Math.Min(1f, num));
        if (num >= 1f)
        {
            Finished = true;
        }
        if (Finished)
        {
            Finish();
        }
    }

    public virtual void Clean()
    {
        _onComplete.Clear();
        Finished = false;
        _started = false;
        _elapsed = 0f;
    }

    public virtual void Reset()
    {
        Finished = false;
        _started = false;
        _elapsed = 0f;
    }
}
