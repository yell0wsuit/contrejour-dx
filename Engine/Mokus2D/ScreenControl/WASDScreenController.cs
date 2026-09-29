using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mokus2D.Util;

namespace Mokus2D.ScreenControl;

public class WASDScreenController : ScreenControllerBase
{
    private readonly Dictionary<Keys, Vector2> _directions = [];

    private bool _scrolling;

    private bool Scrolling
    {
        set
        {
            if (_scrolling != value)
            {
                _scrolling = value;
                if (_scrolling)
                {
                    ScrollStartEvent.Dispatch();
                }
            }
        }
    }

    public event Action ScrollStartEvent;

    public WASDScreenController(IViewScroller scroller = null)
        : base(scroller)
    {
        BindKeys();
    }

    protected virtual void BindKeys()
    {
        AddKey(Keys.W, 0, -1);
        AddKey(Keys.A, -1, 0);
        AddKey(Keys.S, 0, 1);
        AddKey(Keys.D, 1, 0);
        AddKey(Keys.Up, 0, -1);
        AddKey(Keys.Left, -1, 0);
        AddKey(Keys.Down, 0, 1);
        AddKey(Keys.Right, 1, 0);
    }

    protected void AddKey(Keys key, int x, int y)
    {
        _directions[key] = new Vector2(x, y);
        Mokus2DGame.Keyboard.AddListener(key, OnKeyPressed);
    }

    private void OnKeyPressed(Keys key, bool pressed)
    {
        Vector2 vector = _directions[key];
        if (pressed)
        {
            Direction += vector;
        }
        else
        {
            Direction -= vector;
        }
        Scrolling = Direction != Vector2.Zero;
        ScreenScroller?.ScrollSpeed = Direction * Speed;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        foreach (Keys key in _directions.Keys)
        {
            Mokus2DGame.Keyboard.RemoveListener(key, OnKeyPressed);
        }
    }
}
