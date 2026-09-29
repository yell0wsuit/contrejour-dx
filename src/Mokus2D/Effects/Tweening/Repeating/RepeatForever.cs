using Mokus2D.Data;
using Mokus2D.Interfaces;

namespace Mokus2D.Effects.Tweening.Repeating;

public class RepeatForever : ITween, ICleanable, IUpdatable
{
    private static readonly Pool<RepeatForever> Pool = new(() => new RepeatForever());

    private ITween _tween;

    public bool Finished { get; set; }

    public static RepeatForever New(ITween tween)
    {
        return Pool.New().Initialize(tween);
    }

    private RepeatForever()
    {
    }

    private RepeatForever Initialize(ITween tween)
    {
        _tween = tween;
        return this;
    }

    public void Clean()
    {
        _tween = null;
    }

    public void Update(float time)
    {
        _tween.Update(time);
        if (_tween.Finished)
        {
            _tween.Reset();
        }
    }

    public void Free()
    {
        Pool.Free(this);
    }

    public void Reset()
    {
        Finished = false;
    }
}
