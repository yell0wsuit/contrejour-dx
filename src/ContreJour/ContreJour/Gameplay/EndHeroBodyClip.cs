using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJour.Config;
using ContreJour.Gameplay.Eyes;

using FarseerPhysics.Dynamics;

using Mokus2D.Effects.Tween.Easing;
using Mokus2D.Graphics;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class EndHeroBodyClip : HeroBodyClip
    {
        private static readonly float StopOffset = 124f;

        private bool animationsAllowed;

        private readonly List<EnergyPart> energy = [];

        private float energySpeed;

        private bool hasToStop;

        private Outro outro;

        private float shakePosition;

        private bool stoped;

        private MovieStripesView stripesView;

        protected override float FirstRespawnTime => 3f;

        private new bool EyeAnimationsAllowed
        {
            set => base.EyeAnimationsAllowed = value && animationsAllowed;
        }

        public EndHeroBodyClip(LevelBuilderBase builder, object body, Sprite clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            Game.Energy.Blend = BlendMode.Additive;
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
            if (!LevelCompleted)
            {
                base.DoFinish();
            }
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (Finished)
            {
                Tail.Update(time);
            }
            if (hasToStop && !stoped)
            {
                Vector2 linearVelocity = Body.LinearVelocity;
                linearVelocity.X = Math.Min(linearVelocity.X, -0.05f);
                Body.LinearVelocity = linearVelocity;
            }
            if (hasToStop && !stoped && Math.Abs(shakePosition - Clip.Position.X - StopOffset) < 10f && (double)Math.Abs(Body.LinearVelocity.X) < 0.1)
            {
                Body.BodyType = 0;
                Body.LinearVelocity = Vector2.Zero;
                Body.AngularVelocity = 0f;
                stoped = true;
                Sleep = true;
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
            ((EndRoseBodyClip)Builder.GetObject("rose")).DropTear();
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
            Eye.PlayAnimation(new EyeAnimation("McEyeCloseSlow"), force: true);
            Eye.ReturnToDefault = false;
            ((MovieClip)Eye.CurrentBackground).MaxFrame = 18f;
            ((MovieClip)Eye.CurrentBackground).Repeat = false;
            _ = Tail.RotateTo(2f, Tail.RotationRadians - float.DegreesToRadians(70), Cubic.EaseInOut);
            Schedule(ShowOutro, 1f);
        }

        public void LookAtRose()
        {
            SetEyeTargetAngle((float)Math.PI * 3f / 4f);
        }

        private void SmileAfterLook()
        {
            Eye.ViewDistance = 0f;
            Eye.Smile();
            Schedule(ShowOutro, 1f);
        }

        private void ShowOutro()
        {
            outro = new Outro(UserData.Instance.RoseSaved);
            Game.AddChild(outro, 15);
        }

        protected override void FinishLevelSpeed(Vector2 targetPosition, float finishSpeed)
        {
            if (LevelCompleted)
            {
                Game.RestartEnabled = false;
                FinishLevelSpeedEyeAnimation(targetPosition, finishSpeed, null);
            }
            else
            {
                base.FinishLevelSpeed(targetPosition, finishSpeed);
            }
        }

        protected override void FinishReached()
        {
            if (LevelCompleted)
            {
                Actions.ShakeWithDurationPositionOffsetCountScaleDiff(Clip, 8f, Clip.Position, 4f, 50, 0.1f);
                Schedule(AfterShake, 8f);
                CreateLights();
                Game.ZoomToScaleTime(Clip.Position, 1.6f, 10f);
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
            EnergyPart energyPart = new(Game, this, Maths.Random(0f, (float)Math.PI * 2f), Clip.Position);
            energyPart.Collect();
            energyPart.SpeedValue = Math.Min(energySpeed, 600f);
            energySpeed += 10f;
            energy.Add(energyPart);
            Eye.ApplyBonus();
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
            Clip.Tweener.Stop();
            Finished = false;
            Body.BodyType = (BodyType)2;
            Body.SetSensor(value: false);
            hasToStop = true;
            animationsAllowed = false;
            shakePosition = Clip.Position.X;
            EyeAnimationsAllowed = false;
            SoundManager.PlaySound("newClip1", 0.5f);
        }

        protected override void FadeOutTails()
        {
        }
    }
}
