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

    private static readonly EyeAnimation[] ANIMATIONS =
    [
        new("McEyeSmile", null, lockY: true),
        new("McEyeBlink"),
        new("McEyeBlinkOneTime", null, lockY: true),
        new("McEyeWink", null, lockY: true),
        new("McEyeAngry")
    ];

    public readonly EventSender AnimationEndEvent = new();

    public bool ReturnToDefault = true;

    private bool _animationsAllowed = true;

    private Action<IAnimatedNode> clipEndAction;

    private bool isPlaying;

    protected bool HasAnimations => Animations != null && Animations.Length > 0;

    protected virtual EyeAnimation[] Animations => ANIMATIONS;

    protected bool IsWhite => Game != null && Game.WhiteSide;

    protected bool BlackEye => Game != null ? Game.WhiteSide || Game.BlackSide || Game.BonusChapter : false;

    public virtual bool AnimationsAllowed
    {
        get => _animationsAllowed;
        set => _animationsAllowed = value;
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
            _ = this.Schedule(Maths.Random(3f, 10f), Animate);
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
            endDispatcher?.EndEvent += clipEndAction;
        }
    }

    protected virtual void OnAnimation(EyeAnimation animation)
    {
    }

    private void EndAnimation()
    {
        endDispatcher?.EndEvent -= clipEndAction;
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
