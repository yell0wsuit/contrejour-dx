using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Mokus2D.Data;
using Mokus2D.Interfaces;
using Mokus2D.Util;

namespace Mokus2D.Input;

public class KeyboardDelayListener : IUpdatable
{
    private class KeyData : ICleanable
    {
        public float Time;

        public void Clean()
        {
            Time = 0f;
        }
    }

    private static readonly Pool<KeyData> DataPool = new Pool<KeyData>(() => new KeyData());

    private readonly Dictionary<Keys, KeyData> _pressedTimes = new Dictionary<Keys, KeyData>();

    public float RepeatDelay;

    public float RepeatTime;

    public event Action<Keys> KeyPressedEvent;

    public KeyboardDelayListener(KeyboardController controller)
        : this(controller, 0.65f, 0.06f)
    {
    }

    public KeyboardDelayListener(KeyboardController controller, float repeatDelay, float repeatTime)
    {
        RepeatDelay = repeatDelay;
        RepeatTime = repeatTime;
        controller.KeyStateChangedEvent += OnKeyStateChanged;
    }

    private void OnKeyStateChanged(Keys keys, bool pressed)
    {
        if (pressed)
        {
            _pressedTimes.Add(keys, DataPool.New());
            this.KeyPressedEvent.Dispatch(keys);
        }
        else
        {
            KeyData obj = _pressedTimes[keys];
            _pressedTimes.Remove(keys);
            DataPool.Free(obj);
        }
    }

    public void Update(float time)
    {
        foreach (KeyValuePair<Keys, KeyData> pressedTime in _pressedTimes)
        {
            KeyData value = pressedTime.Value;
            value.Time += time;
            if (value.Time > RepeatDelay)
            {
                value.Time -= RepeatTime;
                this.KeyPressedEvent.Dispatch(pressedTime.Key);
            }
        }
    }
}
