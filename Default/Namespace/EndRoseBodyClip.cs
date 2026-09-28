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
    private CosChanger colorChanger;

    private float colorProgress;

    private float colorStep;

    private bool goingDown;

    private float maxTime;

    private MovieClip movie;

    private bool rised;

    private bool saved;

    private float startTime;

    private bool started;

    public EndRoseBodyClip(LevelBuilderBase builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        clip.Scale /= 1.28f;
        Game.BonusTarget = this;
        movie = (MovieClip)this.clip;
        movie.Rewind = true;
        movie.Repeat = false;
        saved = UserData.Instance.RoseSaved;
        if (!saved)
        {
            movie.MaxFrame *= 0.55f;
        }
        maxTime = saved ? 4f : 2.9629629f;
        movie.Color = Color.Black;
        colorChanger = new CosChanger(-0.1f, 0f, 0.05f);
        this.builder.RegisterObject(this, "rose");
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
        return clip.Position + new Vector2(28f, 78f) + (new Vector2(-20f, 20f) * movie.CurrentFrame / movie.MaxFrame);
    }

    private void AddLight()
    {
        Sprite sprite = new McRoseLight
        {
            Position = new Vector2(22f, 114f) + clip.Position,
            Blend = BlendState.Additive,
            OpacityByte = 120
        };
        _ = builder.AddChild(sprite);
        _ = sprite.FadeIn(2f);
    }

    public void ShowLights()
    {
        AddLight();
        AddLight();
    }

    public void DropTear()
    {
        McTear mcTear = new()
        {
            Repeat = false
        };
        _ = builder.AddChild(mcTear);
        mcTear.Position = clip.Position;
        mcTear.Speed = 0.7f;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (startTime != 0f)
        {
            float num = Maths.EaseInOut(Maths.Clamp((Game.TotalTime - startTime) / maxTime, 0f, 1f), movie.MaxFrame);
            movie.CurrentFrame = goingDown ? (movie.MaxFrame - num) : num;
            if (movie.CurrentFrame == movie.MaxFrame)
            {
                if (!saved && !goingDown)
                {
                    goingDown = true;
                    startTime = Game.TotalTime;
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
        startTime = Game.TotalTime;
    }
}
