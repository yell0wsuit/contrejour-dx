using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Data;
using Mokus2D.Input;
using Mokus2D.Platforms.Input;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Exceptions;
using Mokus2D.Visual.Interactive;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Invalidation;

namespace Mokus2D.Visual;

public abstract class AnchorNode : SpriteBatchNode, ITouchDispatchNode, IClickableNode, ITouchNode, IMouseOverNode, IBoundsNode, ISizeNode
{
    public readonly StopPropagationConfig StopPropagation = new StopPropagationConfig();

    public bool TouchOutResult;

    private Vector2 _anchor = new Vector2(0.5f);

    private DirtyProperty<RectangleFloat> _bounds;

    private bool _clickable;

    private bool _clickableAdded;

    private bool _processMouseOver;

    private bool _processMouseOverAdded;

    private bool _isMouseOver;

    public int ClickablePriority { get; set; }

    public bool IsMouseOver
    {
        get
        {
            if (!ProcessMouseOver)
            {
                throw new NodeException("Mouse over is not being processed");
            }
            return _isMouseOver;
        }
    }

    public virtual Vector2 TextureSize { get; protected set; }

    public virtual Vector2 Size => TextureSize * ScaleFactor;

    public Vector2 ScaledSize
    {
        get
        {
            return Size * ScaleVec;
        }
        set
        {
            ScaleVec = value / Size;
        }
    }

    public float ScaledHeight
    {
        get
        {
            return ScaledSize.Y;
        }
        set
        {
            ScaledSize = ScaledSize.ChangeY(value);
        }
    }

    public float ScaledWidth
    {
        get
        {
            return ScaledSize.X;
        }
        set
        {
            ScaledSize = ScaledSize.ChangeX(value);
        }
    }

    public virtual Vector2 Anchor
    {
        get
        {
            return _anchor;
        }
        set
        {
            _anchor = value;
        }
    }

    public float AnchorX
    {
        get
        {
            return Anchor.X;
        }
        set
        {
            Anchor = new Vector2(value, Anchor.Y);
        }
    }

    public float AnchorY
    {
        get
        {
            return Anchor.Y;
        }
        set
        {
            Anchor = new Vector2(Anchor.X, value);
        }
    }

    public Vector2 AnchorInPixels
    {
        get
        {
            return _anchor * Size;
        }
        set
        {
            _anchor = value / Size;
        }
    }

    public bool Clickable
    {
        get
        {
            return _clickable;
        }
        set
        {
            if (value != _clickable)
            {
                _clickable = value;
                RefreshClickableState();
            }
        }
    }

    public bool ProcessMouseOver
    {
        get
        {
            return _processMouseOver;
        }
        set
        {
            if (value != _processMouseOver)
            {
                _processMouseOver = value;
                RefreshProcessMouseOverState();
            }
        }
    }

    public RectangleFloat Bounds
    {
        get
        {
            if (_bounds.TryRefresh())
            {
                _bounds.Value = CalculateBounds();
            }
            return _bounds.Value;
        }
    }

    public event Action<TouchArguments> TouchBeginEvent;

    public event Action<TouchArguments> TouchEndEvent;

    public event Action<TouchArguments> TouchOutEvent;

    public event Action<TouchArguments> TouchMoveEvent;

    public event Action MouseOverEvent;

    public event Action MouseOutEvent;

    protected AnchorNode(Texture2D texture)
        : base(texture)
    {
        _bounds.SetDirty();
    }

    protected void ResetBounds()
    {
        _bounds.SetDirty();
    }

    protected virtual RectangleFloat CalculateBounds()
    {
        float x = (0f - GetScaledAnchor(Anchor.X, base.Root.SpritesScaleFactor.X)) * Size.X;
        float y = (0f - GetScaledAnchor(Anchor.Y, base.Root.SpritesScaleFactor.Y)) * Size.Y;
        return new RectangleFloat(x, y, Size.X, Size.Y);
    }

    private float GetScaledAnchor(float anchor, float scaleFactor)
    {
        if (!(scaleFactor > 0f))
        {
            return 1f - anchor;
        }
        return anchor;
    }

    private void RefreshClickableState()
    {
        bool flag = _clickable && base.Root != null;
        if (flag != _clickableAdded)
        {
            if (flag)
            {
                Mokus2DGame.Instance.SpriteClicksListener.Add(this);
            }
            else
            {
                Mokus2DGame.Instance.SpriteClicksListener.Remove(this);
            }
            _clickableAdded = flag;
        }
    }

    private void RefreshProcessMouseOverState()
    {
        bool flag = ProcessMouseOver && base.Root != null;
        if (flag != _processMouseOverAdded)
        {
            if (flag)
            {
                MouseController.AddMouseOverNode(this);
            }
            else
            {
                MouseController.RemoveMouseOverNode(this);
            }
            _processMouseOverAdded = flag;
        }
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        RefreshClickableState();
        RefreshProcessMouseOverState();
    }

    protected override void OnRemovedFromStage()
    {
        base.OnRemovedFromStage();
        RefreshClickableState();
        RefreshProcessMouseOverState();
    }

    public virtual bool TouchBegin(Touch touch)
    {
        this.TouchBeginEvent.Dispatch(new TouchArguments(touch, this));
        if (StopPropagation.TouchBegin)
        {
            touch.StopPropagation();
        }
        return true;
    }

    public virtual bool TouchMove(Touch touch)
    {
        this.TouchMoveEvent.Dispatch(new TouchArguments(touch, this));
        return true;
    }

    public virtual bool TouchOut(Touch touch)
    {
        this.TouchOutEvent.Dispatch(new TouchArguments(touch, this));
        return TouchOutResult;
    }

    public virtual void TouchEnd(Touch touch)
    {
        this.TouchEndEvent.Dispatch(new TouchArguments(touch, this));
        if (StopPropagation.TouchEnd)
        {
            touch.StopPropagation();
        }
    }

    public void MouseOver()
    {
        _isMouseOver = true;
        this.MouseOverEvent.Dispatch();
    }

    public void MouseOut()
    {
        _isMouseOver = false;
        this.MouseOutEvent.Dispatch();
    }
}
