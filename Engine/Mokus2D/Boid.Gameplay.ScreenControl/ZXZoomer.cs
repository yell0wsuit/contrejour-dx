using System;

using Default.Namespace;

using Microsoft.Xna.Framework.Input;

using Mokus2D;
using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Util.Resources;

namespace Boid.Gameplay.ScreenControl;

public class ZXZoomer : DisposableBase, IUpdatable
{
    private bool Enabled = true;

    private float? ZoomMax;

    private float? ZoomMin;

    private float ZoomSpeed = 1f;

    private float _zoom = 1f;

    private int _zoomDirection;

    public float Zoom
    {
        get => _zoom;
        private set
        {
            if (_zoom != value)
            {
                _zoom = value;
                ZoomChangeEvent.Dispatch(value);
            }
        }
    }

    public event Action<float> ZoomChangeEvent;

    public ZXZoomer()
    {
        Mokus2DGame.Keyboard.AddListener(Keys.Z, OnKeyPressed);
        Mokus2DGame.Keyboard.AddListener(Keys.X, OnKeyPressed);
    }

    public void Update(float time)
    {
        if (Enabled)
        {
            float value = Zoom + (ZoomSpeed * _zoomDirection.Sign() * time);
            value = Maths.Clamp(value, ZoomMin, ZoomMax);
            Zoom = value;
        }
    }

    private void OnKeyPressed(Keys key, bool pressed)
    {
        int num = (key != Keys.Z) ? 1 : (-1);
        if (pressed)
        {
            _zoomDirection += num;
        }
        else
        {
            _zoomDirection -= num;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Mokus2DGame.Keyboard.RemoveListener(Keys.Z, OnKeyPressed);
        Mokus2DGame.Keyboard.RemoveListener(Keys.X, OnKeyPressed);
    }
}
