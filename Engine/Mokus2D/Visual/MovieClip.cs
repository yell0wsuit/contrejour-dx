using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public class MovieClip : MultiframeSprite, IAnimatedNode
{
    private readonly AnimationPlayer _player;

    private int _lastFrame = -1;

    public override Vector2 TextureSize { get; protected set; }

    public bool Rewind
    {
        get => _player.Rewind;
        set => _player.Rewind = value;
    }

    public bool Repeat
    {
        get => _player.Repeat;
        set => _player.Repeat = value;
    }

    public float Speed
    {
        get => _player.Speed;
        set => _player.Speed = value;
    }

    public bool Stoped
    {
        get => _player.Stoped;
        set => _player.Stoped = value;
    }

    public float MinFrame
    {
        get => _player.MinFrame;
        set => _player.MinFrame = value;
    }

    public float MaxFrame
    {
        get => _player.MaxFrame;
        set => _player.MaxFrame = value;
    }

    public int FrameValue => _player.FrameValue;

    public float CurrentFrame
    {
        get => _player.CurrentFrame;
        set => _player.CurrentFrame = value;
    }

    public event Action<IAnimatedNode> EndEvent;

    public MovieClip(string name)
        : this(Mokus2DGame.LoadResource<IMovieClipData>(name))
    {
    }

    public MovieClip(IMovieClipData data)
        : base(data)
    {
        InitializeConfig(data);
        TextureSize = data.Size;
        _player = new AnimationPlayer(this);
        _player.EndEvent += OnEnd;
        Initialize();
    }

    private void OnEnd()
    {
        EndEvent.Dispatch(this);
    }

    public override void ResetData(string id)
    {
        IMovieClipData data = Mokus2DGame.LoadResource<IMovieClipData>(id);
        ResetData(data);
        InitializeConfig(data);
        SetQuadDirty();
        SetTextureRectangleDirty();
    }

    public override void Update(float time)
    {
        base.Update(time);
        _player.Update(time);
        if (_lastFrame != (int)_player.CurrentFrame)
        {
            _lastFrame = (int)_player.CurrentFrame;
            SetTextureRectangleDirty();
            SetQuadDirty();
        }
    }

    protected override Rectangle GetTileRectangle()
    {
        return Frames[_player.FrameValue].Rect;
    }

    protected override Vector2 GetCurrentAnchor()
    {
        return (Anchor + Frames[_player.FrameValue].Anchor) * Size;
    }

    public void Play()
    {
        Stoped = false;
    }

    public void Stop()
    {
        Stoped = true;
    }

    public void GotoAndPlay(float frame)
    {
        _player.GotoAndPlay(frame);
    }

    public void GotoAndStop(float frame)
    {
        _player.GotoAndStop(frame);
    }

    internal override void SetInstanceConfig(IDictionary<string, string> config)
    {
        base.SetInstanceConfig(config);
        AnimationUtil.ApplyAnimationConfig(this);
    }
}
