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
    private const float HERO_EYE_SCALE = 1.07f;

    private const float EYE_CORNER_SPEED = 3f;

    private const float COLOR_TIME = 0.5f;

    private const string EyeBallBlack = "McEyeBallBlack";

    private const string EyeBallWhite = "McEyeBallWhite";

    private const string EyeBallGreen = "McEyeBall_6";

    private static readonly Color START_COLOR = new Color(255, 255, 255);

    private static readonly Color BONUS_COLOR = new Color(143, 238, 255);

    protected bool isDefaultColor;

    protected float colorTime;

    protected float colorProgress;

    protected Dictionary<string, List<string>> sounds;

    protected bool moveAllowed;

    public bool MoveAllowed
    {
        get
        {
            return moveAllowed;
        }
        set
        {
            moveAllowed = value;
            if (!value)
            {
                base.ViewDistance = 0f;
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
        sounds = new Dictionary<string, List<string>>();
        sounds["McEyeSmile"] = new List<string>(new string[3] { "laugh0", "laugh1", "laughl3" });
        sounds["McEyeWink"] = new List<string>(new string[3] { "suspicious0", "suspicious1", "suspicious3" });
        sounds["McEyeAngry"] = new List<string>(new string[1] { "angry2" });
        sounds["McEyeBlinkOneTime"] = new List<string>(new string[1] { "clip0" });
        sounds["McEyeBlink"] = new List<string>(new string[1] { "clip1" });
        if (!base.BlackEye)
        {
            base.Scale = 1.07f;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (moveAllowed)
        {
            base.ViewAngle = VectorUtil.Atan2(velocity);
            base.ViewDistance = velocity.Length() / 3f;
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
            TryPlaySound(animation.EyeBall);
        }
    }

    public bool TryPlaySound(string key)
    {
        if (key != null && sounds.ContainsKey(key))
        {
            SoundManager.PlayRandomSound(sounds[key], key.StartsWith("McEyeBlink") ? 0.3f : 0.75f);
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
        if (!base.BlackEye)
        {
            base.CreateDefaultView();
            return;
        }
        background = new McEyeBlack();
        eyeBall = (Sprite)ClipTypesCache.CreateNewNode(base.Game.ChooseSide("McEyeBallBlack", "McEyeBallWhite", null, null, "McEyeBall_6"));
    }

    protected override string ProcessName(string _name)
    {
        string text = base.ProcessName(_name);
        if (text == "McEyeBallHitBlack")
        {
            if (base.IsWhite)
            {
                return "McEyeBallHitWhite";
            }
            if (base.Game.BonusChapter)
            {
                return "McEyeBallHit_6";
            }
        }
        return text;
    }
}
