using System;
using System.Collections.Generic;

using ContreJour.Clips.common2;
using ContreJour.Content;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

using Mokus2D.Sound;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class HeroEye : RandomAnimationEye
{
    private static readonly Color START_COLOR = new(255, 255, 255);

    private static readonly Color BONUS_COLOR = new(143, 238, 255);

    protected bool isDefaultColor;

    protected float colorTime;

    protected float colorProgress;

    protected Dictionary<string, List<string>> sounds;

    protected bool moveAllowed;

    public bool MoveAllowed
    {
        get => moveAllowed;
        set
        {
            moveAllowed = value;
            if (!value)
            {
                ViewDistance = 0f;
            }
        }
    }

    protected override float ViewRadius => base.ViewRadius * 2f;

    public HeroEye(ContreJourGame game)
        : base(game, useMask: true, new Vector2(50f, 50f))
    {
        moveAllowed = true;
        colorTime = 0f;
        colorProgress = 0f;
        sounds = new Dictionary<string, List<string>>
        {
            ["McEyeSmile"] = ["laugh0", "laugh1", "laughl3"],
            ["McEyeWink"] = ["suspicious0", "suspicious1", "suspicious3"],
            ["McEyeAngry"] = ["angry2"],
            ["McEyeBlinkOneTime"] = ["clip0"],
            ["McEyeBlink"] = ["clip1"]
        };
        if (!BlackEye)
        {
            Scale = 1.07f;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (moveAllowed)
        {
            ViewAngle = VectorUtil.Atan2(velocity);
            ViewDistance = velocity.Length() / 3f;
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        currentBackground.Position = currentEyeBall.Position * 0.5f;
        if (colorTime > 0f)
        {
            colorProgress = Maths.StepTo(colorProgress, 1f, 0.1f);
            isDefaultColor = false;
            colorTime -= time;
        }
        else if (!isDefaultColor)
        {
            colorProgress = Maths.StepTo(colorProgress, 0f, 0.1f);
            if (Maths.FuzzyEquals(colorProgress, 0f))
            {
                isDefaultColor = true;
                RefreshColor(START_COLOR);
            }
        }
        if (!isDefaultColor)
        {
            RefreshColor(Color.Lerp(START_COLOR, BONUS_COLOR, colorProgress));
        }
    }

    protected override void OnAnimation(EyeAnimation animation)
    {
        if (!TryPlaySound(animation.Background))
        {
            _ = TryPlaySound(animation.EyeBall);
        }
    }

    public bool TryPlaySound(string key)
    {
        if (key != null && sounds.TryGetValue(key, out List<string> keySounds))
        {
            SoundManager.PlayRandomSound(keySounds, key.StartsWith("McEyeBlink", StringComparison.Ordinal) ? 0.3f : 0.75f);
            return true;
        }
        return false;
    }

    public void ApplyBonus()
    {
        colorTime = 0.5f;
    }

    public void RefreshColor(Color color)
    {
        if (currentBackground.Parent != null)
        {
            currentBackground.Color = color;
        }
    }

    public void Smile()
    {
        PlayAnimation(new EyeAnimation("McEyeSmile", null, lockY: true), force: true);
    }

    protected override void RefreshLayout()
    {
        base.RefreshLayout();
        isDefaultColor = false;
    }

    protected override void CreateDefaultView()
    {
        if (!BlackEye)
        {
            base.CreateDefaultView();
            return;
        }
        background = new McEyeBlack();
        eyeBall = (Sprite)ClipTypesCache.CreateNewNode(Game.ChooseSide("McEyeBallBlack", "McEyeBallWhite", null, null, "McEyeBall_6"));
    }

    protected override string ProcessName(string name)
    {
        string text = base.ProcessName(name);
        if (text == "McEyeBallHitBlack")
        {
            if (IsWhite)
            {
                return "McEyeBallHitWhite";
            }
            if (Game.BonusChapter)
            {
                return "McEyeBallHit_6";
            }
        }
        return text;
    }
}
