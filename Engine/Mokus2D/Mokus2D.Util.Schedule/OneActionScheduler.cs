using System;

using Mokus2D.Interfaces;

namespace Mokus2D.Util.Schedule;

public class OneActionScheduler(Action action, float timeout) : IUpdatable
{
    private readonly Action _action = action;

    private readonly float Timeout = timeout;

    private float _elapsedTime;

    private readonly bool Enabled = true;

    public float TimeLeft => Timeout - _elapsedTime;

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
