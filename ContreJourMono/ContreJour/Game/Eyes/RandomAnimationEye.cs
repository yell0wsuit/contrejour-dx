using System;

using Default.Namespace;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourMono.ContreJour.Game.Eyes;

public class RandomAnimationEye : EyeBase
{
    private const float MIN_TIMEOUT = 3f;

    private const float MAX_TIMEOUT = 10f;

    private static readonly EyeAnimation[] ANIMATIONS = new EyeAnimation[5]
    {
        new EyeAnimation("McEyeSmile", null, lockY: true),
        new EyeAnimation("McEyeBlink"),
        new EyeAnimation("McEyeBlinkOneTime", null, lockY: true),
        new EyeAnimation("McEyeWink", null, lockY: true),
        new EyeAnimation("McEyeAngry")
    };

    public readonly EventSender AnimationEndEvent = new EventSender();

    public bool ReturnToDefault = true;

    private bool _animationsAllowed = true;

    private Action<IAnimatedNode> clipEndAction;

    private bool isPlaying;

    protected bool HasAnimations
    {
        get
        {
            if (Animations != null)
            {
                return Animations.Length > 0;
            }
            return false;
        }
    }

    protected virtual EyeAnimation[] Animations => ANIMATIONS;

    protected bool IsWhite
    {
        get
        {
            if (base.Game != null)
            {
                return base.Game.WhiteSide;
            }
            return false;
        }
    }

    protected bool BlackEye
    {
        get
        {
            if (base.Game != null)
            {
                if (!base.Game.WhiteSide && !base.Game.BlackSide)
                {
                    return base.Game.BonusChapter;
                }
                return true;
            }
            return false;
        }
    }

    public virtual bool AnimationsAllowed
    {
        get
        {
            return _animationsAllowed;
        }
        set
        {
            _animationsAllowed = value;
        }
    }

    public RandomAnimationEye(ContreJourGame game, bool useMask, Vector2 maskSize)
        : base(game, useMask, maskSize)
    {
        UpdateEnabled = false;
        clipEndAction = OnClipEnd;
        ScheduleAnimation();
        CacheAnimations();
    }

    public RandomAnimationEye(ContreJourGame game)
        : this(game, useMask: false, Vector2.Zero)
    {
    }

    private void CacheAnimations()
    {
    }

    protected virtual void ScheduleAnimation()
    {
        if (HasAnimations)
        {
            this.Schedule(Maths.Random(3f, 10f), Animate);
        }
    }

    protected virtual void Animate()
    {
        if (AnimationsAllowed && HasAnimations)
        {
            PlayAnimation(Animations.RandomItem(), force: false);
        }
        else
        {
            ScheduleAnimation();
        }
    }

    protected override string ProcessName(string name)
    {
        return BlackEye ? (name + "Black") : name;
    }

    public virtual void PlayAnimation(EyeAnimation animation, bool force)
    {
        if (!isPlaying || force)
        {
            if (isPlaying)
            {
                EndAnimation();
            }
            OnAnimation(animation);
            isPlaying = true;
            SetEyeContent(animation);
            if (endDispatcher != null)
            {
                endDispatcher.EndEvent += clipEndAction;
            }
        }
    }

    protected virtual void OnAnimation(EyeAnimation animation)
    {
    }

    private void EndAnimation()
    {
        if (endDispatcher != null)
        {
            endDispatcher.EndEvent -= clipEndAction;
        }
        isPlaying = false;
        ScheduleAnimation();
        if (ReturnToDefault)
        {
            SetDefaultView();
        }
    }

    private void OnClipEnd(IAnimatedNode node)
    {
        EndAnimation();
        AnimationEndEvent.SendEvent();
    }
}
