using System;
using System.Collections.Generic;

using Mokus2D.Interfaces;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual;

public class AnimationPlayer : IUpdatable
{
    private const float EPSILON = 0.0001f;

    private float currentFrame;

    private float maxFrame;

    private float minFrame;

    public float FPS = Mokus2DGame.Config.AnimationFPS;

    private readonly IAnimatedNode _owner;

    public bool SyncToParent { get; set; }

    public bool Rewind { get; set; }

    public bool Repeat { get; set; }

    public float Speed { get; set; }

    public bool Stoped { get; set; }

    public float MinFrame
    {
        get
        {
            return minFrame;
        }
        set
        {
            value = value.Clamp(0f, _owner.TotalFrames - 1);
            if (minFrame != value)
            {
                minFrame = value;
                maxFrame = Math.Max(maxFrame, minFrame + 1f);
                if (currentFrame < minFrame)
                {
                    CurrentFrame = minFrame;
                }
            }
        }
    }

    public float MaxFrame
    {
        get
        {
            return maxFrame;
        }
        set
        {
            value = value.Clamp(1f, _owner.TotalFrames);
            if (maxFrame != value)
            {
                maxFrame = value - 0.0001f;
                if (currentFrame > maxFrame)
                {
                    CurrentFrame = maxFrame;
                }
            }
        }
    }

    public int FrameValue => (int)CurrentFrame;

    public float CurrentFrame
    {
        get
        {
            return currentFrame;
        }
        set
        {
            if (currentFrame != value)
            {
                if ((value > maxFrame || value < minFrame) && !Repeat)
                {
                    value = value.Clamp(minFrame, maxFrame);
                    Stoped = true;
                }
                while (value > maxFrame)
                {
                    value = value - maxFrame + minFrame;
                }
                while (value < minFrame)
                {
                    value = value - minFrame + maxFrame;
                }
                currentFrame = value;
            }
        }
    }

    public event Action EndEvent;

    public AnimationPlayer(IAnimatedNode owner)
    {
        _owner = owner;
        Speed = 1f;
        Repeat = true;
        MaxFrame = owner.TotalFrames;
        SyncToParent = ((Node)owner).Config?.GetBool("syncToParent") ?? false;
    }

    public void Update(float time)
    {
        IAnimatedNode animatedNode = ((Node)_owner).Parent as IAnimatedNode;
        if (SyncToParent && animatedNode != null)
        {
            CurrentFrame = animatedNode.CurrentFrame;
        }
        else if (!Stoped)
        {
            int num = ((!Rewind) ? 1 : (-1));
            float num2 = time * FPS * Speed * (float)num;
            float num3 = (CurrentFrame = currentFrame + num2);
            if ((num3 > maxFrame && !Rewind) || (num3 < minFrame && Rewind))
            {
                this.EndEvent.Dispatch();
            }
        }
    }

    public void GotoAndPlay(float frame)
    {
        Stoped = false;
        CurrentFrame = frame;
    }

    public void GotoAndStop(float frame)
    {
        Stoped = true;
        CurrentFrame = frame;
    }
}
