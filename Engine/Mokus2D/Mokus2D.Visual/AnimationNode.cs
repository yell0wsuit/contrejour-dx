using System;
using System.Collections.Generic;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Collections;
using Mokus2D.Data;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Exceptions;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual;

public abstract class AnimationNode : Node, IAnimatedNode, IConfig, IBoundsNode, ISizeNode
{
    private AnimationData _animationData;

    public bool AnimationEnabled = true;

    public bool TweenEdgeFrames = true;

    internal readonly BiDictionary<string, Node> AnimatedChildren = [];

    private IAnimationNodePlayer _animationPlayer = AnimationNodePlayers.Discrete;

    private readonly AnimationPlayer _player;

    private float _lastFrame = -1f;

    public bool IsChildrenAnimationsDiscrete
    {
        get => _animationPlayer == AnimationNodePlayers.Discrete;
        set => _animationPlayer = value ? AnimationNodePlayers.Discrete : AnimationNodePlayers.Smooth;
    }

    public RectangleFloat Bounds => PrecalculatedBounds;

    public Rectangle PrecalculatedBounds => _animationData.PrecalculatedBounds;

    public Vector2 Size => PrecalculatedBounds.Size();

    public Vector2 ScaledSize
    {
        get => Size * ScaleVec;
        set => ScaleVec = value / Size;
    }

    public ICollection<string> ChildrenNames => AnimatedChildren.Keys;

    public AnimationData AnimationData
    {
        get => _animationData;
        set
        {
            if (_animationData != value)
            {
                _animationData = value;
                ApplyFrameData(Math.Min(CurrentFrame, value.Count - 1));
                AnimationEnabled = value.Count > 1;
                _player.MaxFrame = value.Count;
            }
        }
    }

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
        set
        {
            _player.Speed = value;
            if (value != 1f)
            {
                IsChildrenAnimationsDiscrete = false;
            }
        }
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

    public int TotalFrames => _animationData.Count;

    public float CurrentFrame
    {
        get => _player.CurrentFrame;
        set => _player.CurrentFrame = value;
    }

    public List<AnimationFrameData> CurrentFrameData => _animationData[(int)CurrentFrame];

    public event Action<IAnimatedNode> EndEvent;

    protected AnimationNode(string name)
        : this(Mokus2DGame.LoadResource<AnimationData>(name))
    {
        Name = name;
    }

    protected AnimationNode(AnimationData animationData)
    {
        _animationData = animationData;
        if (animationData.Config != null)
        {
            SetMainConfig(animationData.Config);
        }
        _player = new AnimationPlayer(this);
        _player.EndEvent += OnEnd;
        AnimationEnabled = animationData.Count > 1;
    }

    protected virtual void Initialize()
    {
        foreach (Node child in Children)
        {
            child.OnParentInitialized();
        }
    }

    public AnimationFrameData GetChildFrameData(Node child)
    {
        AnimationFrameData childFrameData = GetChildFrameData(GetChildIndex(child));
        return childFrameData != null && childFrameData.Id == child.Name ? childFrameData : GetChildFrameData(child.Name);
    }

    public AnimationFrameData GetChildFrameData(string childName)
    {
        return AnimationData.GetChildFrameData((int)CurrentFrame, childName);
    }

    public AnimationFrameData GetFirstFrameData(string childName)
    {
        return AnimationData.GetChildFrameData(0, childName);
    }

    public AnimationFrameData GetChildFrameData(int childIndex)
    {
        List<AnimationFrameData> list = AnimationData[(int)CurrentFrame];
        return list.Count > childIndex ? list[childIndex] : null;
    }

    public void ReplaceChild(string childName, Node newChild)
    {
        Node child = GetChild(childName, throwOnNotFound: false);
        int index = Children.Count;
        if (child != null && child.Parent == this)
        {
            index = GetChildIndex(child);
            child.RemoveFromParent();
            newChild.ApplyTransformations(child);
        }
        AddChildAt(newChild, index);
        AnimatedChildren[childName] = newChild;
    }

    public void UnignoreAnimations(string childName)
    {
        Node child = GetChild(childName, throwOnNotFound: false);
        if (child != null)
        {
            GetChild(childName).IgnoredAnimations = IgnoredAnimationProperties.None;
        }
    }

    public void SetDiscreteAnimations(string childName)
    {
        GetChild(childName).IsAnimationDiscrete = true;
    }

    public void IgnoreAnimations(string childName)
    {
        Node child = GetChild(childName, throwOnNotFound: false);
        child?.IgnoredAnimations = IgnoredAnimationProperties.All;
    }

    public void RemoveListeners()
    {
        EndEvent = null;
    }

    private void OnEnd()
    {
        EndEvent.Dispatch(this);
    }

    public string GetChildName(Node child)
    {
        return AnimatedChildren.GetKey(child);
    }

    public Node GetOffspring(string id, bool throwOnNotFound = true)
    {
        string[] array = id.Split(['.']);
        AnimationNode animationNode = this;
        for (int i = 0; i < array.Length - 1; i++)
        {
            animationNode = (AnimationNode)animationNode.GetChild(array[i], throwOnNotFound);
        }
        return animationNode == null && !throwOnNotFound ? null : animationNode.GetChild(array.Last(), throwOnNotFound);
    }

    public bool HasChild(string id)
    {
        return AnimatedChildren.ContainsKey(id);
    }

    public Node GetChild(string id, bool throwOnNotFound = true)
    {
        Node node = AnimatedChildren.TryGetValue(id);
        return node == null && throwOnNotFound ? throw new ChildNotFoundException(id) : node;
    }

    protected override void OnAddedToStage()
    {
        base.OnAddedToStage();
        if (Root.SpritesScaleFactor.X < 0f || Root.SpritesScaleFactor.Y < 0f)
        {
            ApplyFrameData(CurrentFrame);
        }
    }

    protected virtual void AddChild(string id, Node child)
    {
        AnimatedChildren[id] = child;
        child.Name = id;
        AnimationFrameData data = _animationData[0][Children.Count];
        AnimationUtil.ApplyChildFrameData(this, child, data);
        child.SetInstanceConfig(_animationData.GetInstanceConfig(id));
        AddChild(child);
    }

    protected static void SetBlendMode(Node child, string blendMode)
    {
        if (child is SpriteBatchNode spriteBatchNode)
        {
            spriteBatchNode.Blend = BlendStates.GetByName(blendMode);
        }
        foreach (Node child2 in child.Children)
        {
            SetBlendMode(child2, blendMode);
        }
    }

    internal override void SetInstanceConfig(IDictionary<string, string> config)
    {
        base.SetInstanceConfig(config);
        AnimationUtil.ApplyAnimationConfig(this);
        if (Config != null)
        {
            TweenEdgeFrames = Config.GetBool("tweenEdgeFrames", defaultValue: true);
            IsChildrenAnimationsDiscrete = !Config.GetBool("smooth");
        }
    }

    public override void RemoveChild(Node node)
    {
        if (AnimatedChildren.ContainsValue(node))
        {
            _ = AnimatedChildren.Remove(node);
        }
        base.RemoveChild(node);
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (AnimationEnabled)
        {
            _player.Update(time);
            if (!Maths.FuzzyEquals(_lastFrame, _player.CurrentFrame))
            {
                ApplyFrameData(_player.CurrentFrame);
                _lastFrame = _player.CurrentFrame;
            }
        }
    }

    public void ApplyCurrentFrame()
    {
        ApplyFrameData(CurrentFrame);
    }

    public void ApplyCurrentFrame(Node child)
    {
        AnimationUtil.ApplyChildFrameData(this, child, GetChildFrameData(child.Name));
    }

    protected void ApplyFrameData(float frame)
    {
        _animationPlayer.ApplyFrameData(this, frame);
    }

    public void Play()
    {
        Stoped = false;
    }

    public void Stop()
    {
        Stoped = true;
    }

    public void ShowAndPlayFromStart()
    {
        VisibleAndUpdating = true;
        PlayFromStart();
    }

    public void PlayFromStart()
    {
        Rewind = false;
        GotoAndPlay(0f);
    }

    public void PlayFromEnd()
    {
        Rewind = true;
        GotoAndPlay(TotalFrames - 1);
    }

    public void GotoAndPlay(float frame)
    {
        _player.GotoAndPlay(frame);
    }

    public void GotoAndStop(float frame)
    {
        _player.GotoAndStop(frame);
    }

    public override void ReplaceChild(Node child, Node replacement)
    {
        base.ReplaceChild(child, replacement);
        if (child.Name != null)
        {
            replacement.Name = child.Name;
            AnimatedChildren[child.Name] = replacement;
        }
    }
}
