using System;

using ContreJour.Clips.chapter5;
using ContreJour.Clips.menu2;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EndRoseBodyClip : StickyBodyClip, IBonusAcceptable, IBodyClip
{
    protected CosChanger colorChanger;

    protected float colorProgress;

    protected float colorStep;

    protected bool goingDown;

    protected float maxTime;

    protected MovieClip movie;

    protected bool rised;

    protected bool saved;

    protected float startTime;

    protected bool started;

    public EndRoseBodyClip(LevelBuilderBase _builder, object _body, Sprite _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        _clip.Scale /= 1.28f;
        base.Game.BonusTarget = this;
        movie = (MovieClip)clip;
        movie.Rewind = true;
        movie.Repeat = false;
        saved = UserData.Instance.RoseSaved;
        if (!saved)
        {
            movie.MaxFrame *= 0.55f;
        }
        maxTime = (saved ? 4f : 2.9629629f);
        movie.Color = Color.Black;
        colorChanger = new CosChanger(-0.1f, 0f, 0.05f);
        builder.RegisterObject(this, "rose");
    }

    public void ApplyBonus()
    {
        if (!started)
        {
            started = true;
            Schedule(Start, saved ? 5 : 6);
        }
        colorStep = Math.Max((Math.Abs(movie.CurrentFrame - movie.MaxFrame) < 8f) ? 0.03f : 0.01f, colorStep + 0.003f);
    }

    public Vector2 BonusTarget()
    {
        return clip.Position + new Vector2(28f, 78f) + new Vector2(-20f, 20f) * movie.CurrentFrame / movie.MaxFrame;
    }

    private void AddLight(float direction)
    {
        Sprite sprite = new McRoseLight();
        sprite.Position = new Vector2(22f, 114f) + clip.Position;
        sprite.Blend = BlendState.Additive;
        sprite.OpacityByte = 120;
        builder.AddChild(sprite);
        sprite.FadeIn(2f);
    }

    public void ShowLights()
    {
        AddLight(-1f);
        AddLight(1f);
    }

    public void DropTear()
    {
        McTear mcTear = new McTear();
        mcTear.Repeat = false;
        builder.AddChild(mcTear);
        mcTear.Position = clip.Position;
        mcTear.Speed = 0.7f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (startTime != 0f)
        {
            float num = Maths.EaseInOut(Maths.Clamp((base.Game.TotalTime - startTime) / maxTime, 0f, 1f), movie.MaxFrame);
            movie.CurrentFrame = (goingDown ? (movie.MaxFrame - num) : num);
            if (movie.CurrentFrame == movie.MaxFrame)
            {
                if (!saved && !goingDown)
                {
                    goingDown = true;
                    startTime = base.Game.TotalTime;
                }
                else if (saved && !rised)
                {
                    rised = true;
                    XBoxUtil.AwardAchievement("little_prince");
                }
            }
        }
        if (!goingDown)
        {
            if (rised)
            {
                colorStep = 0.05f;
            }
            else
            {
                if (Math.Abs(movie.CurrentFrame - movie.MaxFrame) > 8f)
                {
                    colorStep -= 0.0005f;
                }
                colorStep = Maths.Clamp(colorStep, -0.05f, 0.035f);
            }
            colorProgress = Maths.Clamp(colorProgress + colorStep, 0f, 1f);
        }
        else
        {
            colorProgress -= 0.002f;
        }
        colorChanger.Update(time);
        movie.Color = Color.White * Maths.Clamp(colorChanger.Value + colorProgress, 0f, 0.98f);
    }

    private void Start()
    {
        startTime = base.Game.TotalTime;
    }
}
