using System;
using System.Collections.Generic;

using ContreJour.Config;

using ContreJourMono.ContreJour.Game.Eyes;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Default.Namespace;

public class EndHeroBodyClip : HeroBodyClip
{
    private static readonly float STOP_OFFSET = 124f;

    protected bool animationsAllowed;

    protected List<EnergyPart> energy = [];

    protected float energySpeed;

    protected bool hasToStop;

    protected Outro outro;

    protected float shakePosition;

    protected bool stoped;

    protected MovieStripesView stripesView;

    protected override float FirstRespawnTime => 3f;

    private new bool EyeAnimationsAllowed
    {
        set => base.EyeAnimationsAllowed = value && animationsAllowed;
    }

    public EndHeroBodyClip(LevelBuilderBase builder, object body, Sprite clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        Game.Energy.Blend = BlendState.Additive;
        EyeAnimationsAllowed = false;
        animationsAllowed = true;
        Game.BackEvent.AddListener(OnBack);
    }

    public void AddStripesView()
    {
        if (ContreJourConfig.BackButtonVisible)
        {
            stripesView = new BackMovieStripes();
            ((BackMovieStripes)stripesView).BackEvent.AddListener(Game.Back);
        }
        else
        {
            stripesView = new MovieStripesView(blackSide: false, fade: false);
        }
        Game.AddView(stripesView);
        Game.HidePause();
        stripesView.Show();
    }

    private void OnBack()
    {
        outro?.Dispose();
    }

    protected override void DoFinish()
    {
        if (!levelCompleted)
        {
            base.DoFinish();
        }
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (finished)
        {
            tail.Update(time);
        }
        if (hasToStop && !stoped)
        {
            Vector2 linearVelocity = Body.LinearVelocity;
            linearVelocity.X = Math.Min(linearVelocity.X, -0.05f);
            Body.LinearVelocity = linearVelocity;
        }
        if (hasToStop && !stoped && Math.Abs(shakePosition - clip.Position.X - STOP_OFFSET) < 10f && (double)Math.Abs(Body.LinearVelocity.X) < 0.1)
        {
            Body.BodyType = 0;
            Body.LinearVelocity = Vector2.Zero;
            Body.AngularVelocity = 0f;
            stoped = true;
            sleep = true;
            EyeMoveAllowed = false;
            SoundManager.PlaySound("petitkoIsHoping", 0.3f);
            Schedule(LookAtRose, 0.2f);
            if (UserData.Instance.RoseSaved)
            {
                Schedule(SmileAfterLook, 2f);
            }
            else
            {
                Schedule(LookAtTear, 1.5f);
            }
        }
    }

    private void LookAtTear()
    {
        ((EndRoseBodyClip)builder.GetObject("rose")).DropTear();
        Schedule(LookAfterTear, 1f);
    }

    private void LookAfterTear()
    {
        SetEyeTargetAngle(3.7699113f);
        Schedule(LookAtRose2, 1.5f);
    }

    private void LookAtRose2()
    {
        LookAtRose();
        Schedule(BecomeSad, 2f);
    }

    private void BecomeSad()
    {
        SetEyeTargetAngle(4.712389f);
        SoundManager.PlaySound("saddness", 0.8f);
        eye.PlayAnimation(new EyeAnimation("McEyeCloseSlow"), force: true);
        eye.ReturnToDefault = false;
        ((MovieClip)eye.CurrentBackground).MaxFrame = 18f;
        ((MovieClip)eye.CurrentBackground).Repeat = false;
        _ = tail.RotateTo(2f, tail.RotationRadians - 70.ToRadians(), Cubic.EaseInOut);
        Schedule(ShowOutro, 1f);
    }

    public void LookAtRose()
    {
        SetEyeTargetAngle((float)Math.PI * 3f / 4f);
    }

    private void SmileAfterLook()
    {
        eye.ViewDistance = 0f;
        eye.Smile();
        Schedule(ShowOutro, 1f);
    }

    private void ShowOutro()
    {
        outro = new Outro(UserData.Instance.RoseSaved);
        Game.AddChild(outro, 15);
    }

    protected override void FinishLevelSpeed(Vector2 targetPosition, float _finishSpeed)
    {
        if (levelCompleted)
        {
            Game.RestartEnabled = false;
            FinishLevelSpeedEyeAnimation(targetPosition, _finishSpeed, null);
        }
        else
        {
            base.FinishLevelSpeed(targetPosition, _finishSpeed);
        }
    }

    protected override void FinishReached()
    {
        if (levelCompleted)
        {
            Actions.ShakeWithDurationPositionOffsetCountScaleDiff(clip, 8f, clip.Position, 4f, 50, 0.1f);
            Schedule(AfterShake, 8f);
            CreateLights();
            Game.ZoomToScaleTime(clip.Position, 1.6f, 10f);
            Game.TouchEnabled = false;
            animationsAllowed = false;
            EyeAnimationsAllowed = false;
            AddStripesView();
            Game.RenewGround();
            Schedule(PlayShakeSound, 7f);
        }
    }

    private void PlayShakeSound()
    {
        SoundManager.PlaySound("petitkoIsTrying", 0.3f);
    }

    public void CreateLights()
    {
        float num = UserData.Instance.TotalStars * (1f / 3f);
        for (int i = 1; i < num; i++)
        {
            Schedule(delay: 8f * (float)Math.Sin(i / num * ((float)Math.PI / 2f)), action: CreateLight);
        }
        energySpeed = 200f;
    }

    private void CreateLight()
    {
        EnergyPart energyPart = new(Game, this, Maths.Random(0f, (float)Math.PI * 2f), clip.Position);
        energyPart.Collect();
        energyPart.SpeedValue = Math.Min(energySpeed, 600f);
        energySpeed += 10f;
        energy.Add(energyPart);
        eye.ApplyBonus();
    }

    protected override void UpdateShadow(float time)
    {
        if (!stoped)
        {
            base.UpdateShadow(time);
        }
        else
        {
            UpdateShadowOpacityTime(hasShadow: true, time);
        }
    }

    private void AfterShake()
    {
        clip.Tweener.Stop();
        finished = false;
        Body.BodyType = (BodyType)2;
        Body.SetSensor(value: false);
        hasToStop = true;
        animationsAllowed = false;
        shakePosition = clip.Position.X;
        EyeAnimationsAllowed = false;
        SoundManager.PlaySound("newClip1", 0.5f);
    }

    protected override void FadeOutTails()
    {
    }
}
