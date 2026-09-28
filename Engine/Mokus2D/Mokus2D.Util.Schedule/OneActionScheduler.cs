using System;

using Mokus2D.Interfaces;

namespace Mokus2D.Util.Schedule;

public class OneActionScheduler : IUpdatable
{
    private readonly Action _action;

    public float Timeout;

    private float _elapsedTime;

    public bool Enabled = true;

    public float TimeLeft => Timeout - _elapsedTime;

    public OneActionScheduler(Action action, float timeout)
    {
        _action = action;
        Timeout = timeout;
    }

    public void Reset()
    {
        _elapsedTime = 0f;
    }

    public void Update(float time)
    {
        if (Enabled)
        {
            _elapsedTime += time;
            while (_elapsedTime >= Timeout)
            {
                _elapsedTime -= Timeout;
                _action();
            }
        }
    }
}
