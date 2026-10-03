using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJour.Clips;
using ContreJour.Content;
using ContreJour.Gameplay.Eyes;
using ContreJour.Gameplay.Hero;
using ContreJour.Utils;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Mokus2D.Events;
using Mokus2D.Graphics;
using Mokus2D.Integration.Farseer.Physics;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class HeroBodyClip : ContreJourBodyClip, IVectorPositionProvider, IBonusAcceptable, ISnotLinked, IEatable, ISpikesDestroyable, ILaunchable, IRadius, IBodyClip, ITeleportable, IRestartable
    {
        public EventSender FinishEvent { get; } = new();

        private readonly float eyeScale;

        private readonly List<BlackTail> blackTails;

        private readonly Sprite bodyBackground;

        private float breatheScale;

        private float breatheScaleStep;

        private bool disablePositionUpdate;

        private bool eating;

        public HeroEye Eye { get; }
        private bool eyeClosed;

        private float finishPause;

        private Vector2 finishPosition;

        private bool finishSet;

        private float finishSpeed;

        protected bool Finished { get; set; }

        private bool hasToYawn;
        private readonly Sprite hotspot;

        private Vector2 initialPosition;

        private float lastFootPosition;

        private float lastHitTime;

        private float lastOnGroundTime;

        protected bool LevelCompleted { get; set; }

        private bool migthyPosted;

        private bool onGround;

        private float onGroundTime;

        private bool onPlasticine;

        private readonly Portal portal;

        private Vector2 previousSpeed;
        private bool restartOnEating;

        private bool restarting;

        private readonly Sprite shadow;

        protected bool Sleep { get; set; }

        private float sleepSoundTime;
        private bool speedyPosted;

        protected HeroTail Tail { get; set; }

        private Vector2 targetScale;

        protected ref Vector2 TargetScale => ref targetScale;
        private float timeToSleep;

        private float velocity;

        private Vector2 worldSize;

        public EventSender TeleportEvent => DestroyEvent;

        public bool Removed { get; set; }

        public bool SpeedLocked { get; set; }

        protected virtual float FirstRespawnTime => 0.5f;

        public virtual bool EyeAnimationsAllowed
        {
            get => Eye.AnimationsAllowed;
            set => Eye.AnimationsAllowed = value;
        }

        public bool EyeMoveAllowed
        {
            get => Eye.MoveAllowed;
            set => Eye.MoveAllowed = value;
        }

        public bool HitEnabled { get; set; }

        public int SnotJoinedCount
        {
            get; set
            {
                if (value != field)
                {
                    field = value;
                    if (field >= 6)
                    {
                        XBoxUtil.AwardAchievement("spider");
                    }
                }
            }
        }

        public bool SnotEnabled { get; set; }

        public EventSender DestroyEvent { get; }

        public override Vector2 PositionVec => Body.Position;

        public HeroBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            Body.IsBullet = true;
            if (Game.BlackSide || Game.WhiteSide || Game.BonusChapter)
            {
                bodyBackground = (Sprite)ClipCatalog.Create(Game.ChooseSide("McHeroBlackView", "McHeroWhiteView", "McHeroBackView", "McHeroBackView", "McHeroView_6"));
                LevelBuilderBase.ReplaceChildWith(clip, bodyBackground);
            }
            else
            {
                bodyBackground = (Sprite)clip;
            }
            Vector2 position = clip.Position;
            bodyBackground.Position = Vector2.Zero;
            clip = new Node();
            Clip = clip;
            clip.Position = position;
            shadow = (Sprite)ClipCatalog.Create(Game.ChooseSide("McHeroShadow", "McHeroShadowWhite", "McHeroShadow", "McHeroShadow"));
            clip.AddChild(shadow);
            builder.Add(clip, 10);
            bodyBackground.Parent.RemoveChild(bodyBackground);
            clip.AddChild(bodyBackground);
            worldSize = builder.ToVec(Game.LevelSize);
            initialPosition = Body.Position;
            Body.SleepingAllowed = false;
            Body.SetGroupIndex(-2);
            lastHitTime = 0f;
            SnotEnabled = true;
            InitializeBody();
            HitEnabled = true;
            if (Game.LevelIndex != 0 || !Game.CanShowIntro)
            {
                InitializeBody();
                Schedule(FirstRespawn, FirstRespawnTime);
                if (Game.EndLevel != null)
                {
                    Schedule(Game.EndLevel.ShowPortal, 1.6f);
                }
                portal = new Portal(Game, clip.Position);
                Builder.AddChildBefore(portal, Clip);
                portal.ItemsScale = 0f;
                portal.ScaleStep = 0.1f;
            }
            onGroundTime = 0f;
            Config["hasDust"] = true;
            hotspot = new Sprite(ClipIds.Common.McHotspotwhite);
            clip.AddChild(hotspot);
            Eye = new HeroEye(Game);
            eyeScale = Eye.Scale;
            clip.AddChild(Eye);
            if (Game.BlackSide || Game.BonusChapter)
            {
                blackTails = [];
                BlackTail blackTail = new(this);
                blackTails.Add(blackTail);
                Builder.Add(blackTail, 3);
            }
            else
            {
                Tail = new HeroTail(Game.WhiteSide ? ContreJourConstants.WhiteTailColor : Color.Black);
                clip.AddChild(Tail, -1);
            }
            breatheScale = 0f;
            Sleep = false;
            timeToSleep = 7f;
            eyeClosed = false;
            Game.RegisterHero(this);
            onGround = false;
            TargetScale = new Vector2(1f, 1f);
            breatheScaleStep = 0f;
            DestroyEvent = new EventSender();
            ContreJourGame.AddShadowSource();
        }

        public Vector2 BonusTarget()
        {
            return Clip.Position;
        }

        public void ApplyBonus()
        {
            Eye.ApplyBonus();
        }

        public override void UpdatePosition()
        {
            //IL_000e: Unknown result type (might be due to invalid IL or missing references)
            if (!disablePositionUpdate && Body.BodyType != 0)
            {
                base.UpdatePosition();
            }
        }

        public bool CanDie()
        {
            return !restarting && SnotEnabled;
        }

        public float DeadEyeScale()
        {
            return 1f;
        }

        public void EatSpeedPauseScaleTime(Vector2 targetPosition, float finishSpeed, float pause, float scale, float time)
        {
            Game.Amie?.ReleaseHero();
            eating = true;
            FailLevelSpeedPause(targetPosition, finishSpeed, pause);
            SetScaleTime(scale, time);
        }

        public bool CanLaunch()
        {
            return true;
        }

        public float Radius()
        {
            return 5f / 6f;
        }

        public void SetSpeedLocked(bool value)
        {
            SpeedLocked = value;
            Eye.MoveAllowed = !value;
        }

        public void Restart()
        {
            Game.Amie?.ReleaseHero();
            restarting = true;
            FinishEvent.SendEvent();
            restartOnEating = eating;
            DestroyEvent.SendEvent();
            FadeOutTails();
            _ = (Tail?.FadeOutAndHide(0.1f));
            Schedule(HideBody, 0.1f);
        }

        public void Explode()
        {
            Game.Amie?.ReleaseHero();
            eating = true;
            UserData.Instance.Accupuncture++;
            SoundManager.PlayRandomSound(Sounds.DeathBySpikes, 0.7f);
            FailLevelSpeedPauseEyeAnimation(Body.Position, 1f, 1f, null);
            UpdatePosition();
            disablePositionUpdate = true;
            new HeroExplosion().Explode(this, Game);
        }

        public void DoExplode()
        {
            if (!restarting)
            {
                float num = 0.15f;
                _ = bodyBackground.FadeOut(num);
                HideEye();
                _ = hotspot.FadeOut(num);
                Schedule(Hide, num);
                Tail?.Visible = false;
            }
        }

        public bool CanTeleport()
        {
            return CanDie() && !eating;
        }

        public void Teleport(BodyClip teleport)
        {
            TeleportTail(teleport.Body.Position);
            TeleportEvent.SendEvent();
        }

        public void AfterTeleport()
        {
            if (!restarting)
            {
                NewBlackTail();
            }
        }

        public void ForceClipPosition()
        {
            base.UpdatePosition();
        }

        public void SetScaleTime(float scale, float time)
        {
            Clip.ScaleY = Clip.ScaleX;
            _ = Clip.ScaleTo(time, scale);
        }

        private void HideBody()
        {
            _ = Clip.FadeOut(0.2f);
            Schedule(Respawn, 0.2f);
        }

        public void InitializeBody()
        {
            Body.BodyType = 0;
            Clip.Visible = false;
            Body.SetDensity(0.1f);
        }

        public bool OnGround()
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            //IL_0027: Invalid comparison between Unknown and I4
            for (ContactEdge val = Body.ContactList; val != null; val = val.Next)
            {
                if (val.Contact.IsTouching && (int)val.Other.BodyType != 2 && !val.Other.IsSensor())
                {
                    return true;
                }
            }
            return false;
        }

        public void SetEyeTargetAngle(float value)
        {
            Eye.ViewAngle = value;
            Eye.ViewDistance = 0.7f;
        }

        public void SetPosition(Vector2 position)
        {
            Clip.Position = position;
            Body.SetTransform(Builder.ToIPhoneVec(position), Body.Rotation);
        }

        private void FirstRespawn()
        {
            if (!restarting)
            {
                Respawn();
            }
        }

        public void Respawn()
        {
            bodyBackground.OpacityFloat = 1f;
            bodyBackground.Color = Color.White;
            TeleportTail(Body.Position);
            Builder.ChangeChildLayer(Clip, 10);
            EyeAnimationsAllowed = true;
            Eye.Visible = true;
            Eye.SetDefaultView();
            if (Tail != null)
            {
                Tail.Tweener.Stop();
                Tail.Scale = 1f;
                Tail.Visible = true;
                Tail.OpacityFloat = 1f;
            }
            Body.SetSensor(value: false);
            Sleep = false;
            finishSet = false;
            disablePositionUpdate = false;
            Finished = false;
            InitializeBody();
            restarting = false;
            Clip.OpacityByte = 255;
            Body.SetTransform(initialPosition, Body.Rotation);
            ForceClipPosition();
            portal.TargetScale = 1f;
            Schedule(ShowClip, 0.7f);
            SoundManager.PlaySound("begin5", 0.5f);
            NewBlackTail();
        }

        private void ShowClip()
        {
            Clip.Tweener.Stop();
            Clip.OpacityFloat = 1f;
            Clip.Visible = true;
            Clip.Scale = 0f;
            _ = Clip.ScaleTo(0.2f, 1f);
            Schedule(StartPlay, 0.2f);
        }

        private void StartPlay()
        {
            Body.BodyType = (BodyType)2;
            Schedule(RemovePortal, 0.2f);
        }

        private void RemovePortal()
        {
            portal.TargetScale = 0f;
        }

        public override void Update(float time)
        {
            //IL_01c1: Unknown result type (might be due to invalid IL or missing references)
            //IL_03a1: Unknown result type (might be due to invalid IL or missing references)
            //IL_03a7: Invalid comparison between Unknown and I4
            base.Update(time);
            if (hotspot != null && !finishSet)
            {
                hotspot.OpacityByte = (int)(Game.LightPower * 150f);
                hotspot.RotationRadians = VectorUtil.Atan2(Body.Position, Game.LightPoint);
            }
            if (blackTails != null)
            {
                int num = 0;
                List<object> list = [];
                foreach (BlackTail blackTail in blackTails)
                {
                    blackTail.UpdateNode(time);
                    if (num != 0 && blackTail.Length <= 1)
                    {
                        Builder.RemoveChild(blackTail);
                        list.Add(blackTail);
                    }
                    num++;
                }
                blackTails.RemoveList(list);
            }
            if (finishSet)
            {
                Vector2 vector = VectorUtil.StepTo(Body.Position, finishPosition, finishSpeed);
                Body.SetTransform(vector, Body.Rotation);
                ForceClipPosition();
                if (vector == finishPosition)
                {
                    finishSet = false;
                    FinishReached();
                }
            }
            onPlasticine = Body != null && Body.IsTouching(typeof(PlasticinePartBodyClip));
            onGround = Body != null && OnGround();
            if (onGround)
            {
                lastOnGroundTime = Game.TotalTime;
            }
            Body.AngularDamping = (Game.TotalTime - lastOnGroundTime > 0.3f) ? 0.5f : 0f;
            if (Body != null && Body.BodyType == 0 && !onGround && SnotJoinedCount == 0)
            {
            }
            else
            {
            }
            if (onPlasticine)
            {
                onGroundTime += time;
            }
            else
            {
                onGroundTime = 0f;
            }
            TryConfuse();
            UpdateShadow(time);
            float num2 = VectorUtil.Atan2(Body.LinearVelocity);
            float num3 = SpeedLocked ? 0f : Body.LinearVelocity.Length();
            if (!speedyPosted && onGroundTime > 0.2f && num3 >= 11.666667f && HitEnabled)
            {
                XBoxUtil.AwardAchievement("speedy");
                speedyPosted = true;
            }
            if (!migthyPosted && !onGround && SnotJoinedCount == 0 && num3 >= 33.333332f && HitEnabled)
            {
                XBoxUtil.AwardAchievement("mighty_bird");
                migthyPosted = true;
            }
            float num4 = (SnotJoinedCount > 0) ? 33.333332f : 66.666664f;
            if (num3 > num4)
            {
                num3 = num4;
                Body.LinearVelocity = VectorUtil.ToVector(num4, num2);
            }
            Eye.SetVelocity(Body.LinearVelocity);
            Eye.UpdateNode(time);
            if (!Finished && Tail != null)
            {
                Tail.LimitAngles = onGround;
                Tail.SetMovementDirection(num2);
                Tail.Speed = num3;
                Tail.UpdateSpeed = Sleep ? 0.2f : 1f;
            }
            previousSpeed = Body.LinearVelocity;
            velocity = num3;
            TryWakeUp();
            if ((int)Body.BodyType == 2 && Math.Abs(Clip.ScaleY - 1f) < 0.04f && Body.JointList == null)
            {
                TryBreathe(time);
                TrySleep();
                CheckOutOffLevel();
                CheckOutOfBorder();
            }
        }

        protected virtual void FinishReached()
        {
        }

        public void TryConfuse()
        {
            float num = Vector2.Distance(Body.LinearVelocity, previousSpeed);
            bool flag = Game.TotalTime - lastOnGroundTime < 0.1f;
            if (!(num >= 4f) || finishSet || SpeedLocked)
            {
                return;
            }
            bool flag2 = false;
            if (Game.TotalTime - lastHitTime > 0.5f && HitEnabled && flag && Math.Abs(Body.LinearVelocity.Y) < Math.Abs(previousSpeed.Y) && Body.LinearVelocity.Length() < 5f)
            {
                flag2 = true;
                SoundManager.PlaySound("landing1", Math.Min(num / 4f / 3f, 1f) / 10f);
                lastHitTime = Game.TotalTime;
            }
            if (Math.Abs(Body.LinearVelocity.X - previousSpeed.X) >= 4f && Body.LinearVelocity.X <= 1f)
            {
                if (!flag2 && flag && HitEnabled && Game.TotalTime - lastHitTime > 0.5f)
                {
                    SoundManager.PlaySound("landing1", Math.Min(num / 4f / 3f, 1f) / 10f);
                    lastHitTime = Game.TotalTime;
                }
                Eye.PlayAnimation(new EyeAnimation(null, "McEyeBallHit", lockY: true, lockX: true), force: false);
                Eye.AnimationEndEvent.AddListener(OnHitEnd);
            }
        }

        public void PlayFootSound()
        {
            if (onGround)
            {
                bool flag = Game.TotalTime - lastOnGroundTime < 0.2f;
                if (!flag)
                {
                    lastFootPosition = Body.Position.X;
                }
                else if (flag && Math.Abs(Body.Position.X - lastFootPosition) > 1f)
                {
                    lastFootPosition = Body.Position.X;
                    SoundManager.PlayRandomSound(Sounds.FootSteps);
                }
                lastOnGroundTime = Game.TotalTime;
            }
        }

        protected virtual void UpdateShadow(float time)
        {
            //IL_00a0: Unknown result type (might be due to invalid IL or missing references)
            //IL_00ad: Unknown result type (might be due to invalid IL or missing references)
            //IL_00b3: Invalid comparison between Unknown and I4
            ContactEdge val = Body.ContactList;
            bool flag = false;
            float num = 0f;
            int num2 = 0;
            while (val != null)
            {
                Vector2 worldPoint = FarseerUtil.GetWorldPoint(val.Contact);
                BodyClip bodyClip = (BodyClip)val.Other.UserData;
                if (bodyClip != null && val.Contact.IsTouching && !val.Contact.FixtureA.IsSensor && !val.Contact.FixtureB.IsSensor && worldPoint.Y - Body.Position.Y < -7f / 12f && (val.Other.BodyType == 0 || (int)val.Other.BodyType == 1) && bodyClip.Config != null && !bodyClip.Config.ContainsKey("noShadow"))
                {
                    flag = true;
                    num += (worldPoint.X - Body.Position.X) / Builder.EngineConfig.SizeMultiplier;
                    num2++;
                }
                val = val.Next;
            }
            if (flag)
            {
                float num3 = num / num2;
                shadow.Position = VectorUtil.StepTo(shadow.Position, new Vector2(num3, 0f), Math.Min(1f, Math.Abs(num3 / 10f)));
            }
            UpdateShadowOpacityTime(flag, time);
        }

        protected void UpdateShadowOpacityTime(bool hasShadow, float time)
        {
            shadow.OpacityByte = (int)(Maths.StepTo(maxStep: 20f * time * 30f, value: shadow.OpacityByte, target: hasShadow ? bodyBackground.OpacityByte : 0) * Game.LightPower);
            shadow.Visible = shadow.OpacityByte > 0;
        }

        public void CheckOutOfBorder()
        {
            Vector2 position = Body.Position;
            if (position.X < 5f / 6f)
            {
                Body.SetTransform(new Vector2(5f / 6f, Body.Position.Y), Body.Rotation);
            }
            if (position.X > worldSize.X - (5f / 6f))
            {
                Body.SetTransform(new Vector2(worldSize.X - (5f / 6f), Body.Position.Y), Body.Rotation);
            }
            if (position.Y > worldSize.Y - (5f / 6f))
            {
                Body.SetTransform(new Vector2(Body.Position.X, worldSize.Y - (5f / 6f)), Body.Rotation);
            }
        }

        public void CheckOutOffLevel()
        {
            if (Body.Position.Y < -3f)
            {
                // The iOS build plays this; the Windows 8 port dropped it, so falling out was silent.
                SoundManager.PlayRandomSound(Sounds.DeathByFall);
                FailLevelSpeedPause(Body.Position, 0f, 0f);
                UserData.Instance.OutOfScreen++;
            }
        }

        public void TryBreathe(float time)
        {
            //IL_0021: Unknown result type (might be due to invalid IL or missing references)
            if (velocity < 0.1f && !finishSet && Body.BodyType != 0)
            {
                timeToSleep -= time;
                EyeAnimationsAllowed = false;
                if (Sleep && eyeClosed)
                {
                    breatheScaleStep += 0.02f;
                    breatheScaleStep = breatheScaleStep.SimplifyAngle(0f);
                    float num = Maths.Cos(breatheScaleStep);
                    breatheScale = num * 0.04f;
                    TargetScale.X = 1f + breatheScale;
                    TargetScale.Y = 1f + (breatheScale / 2f);
                    MovieClip movieClip = (MovieClip)Eye.CurrentBackground;
                    int num2 = (int)(movieClip.TotalFrames * (1f + num) / 2f);
                    movieClip.GotoAndStop((int)Maths.StepTo(movieClip.CurrentFrame, num2, 1f));
                    sleepSoundTime += time;
                    if (num2 == 12 && breatheScaleStep > (float)Math.PI && sleepSoundTime >= 1.5f)
                    {
                        sleepSoundTime = 0f;
                        hasToYawn = Maths.Random() > 0.6f;
                        if (hasToYawn)
                        {
                            SoundManager.PlaySound("breathIn4", 0.27f);
                        }
                    }
                    else if (num2 == 23 && breatheScaleStep < (float)Math.PI && hasToYawn)
                    {
                        hasToYawn = false;
                        SoundManager.PlayRandomSound(Sounds.SLEEPING, 0.5f);
                    }
                }
            }
            else
            {
                EyeAnimationsAllowed = true;
                TargetScale.X = TargetScale.Y = 1f;
                timeToSleep = 7f;
            }
            Clip.ScaleX = Maths.StepTo(Clip.ScaleX, TargetScale.X, 0.0005f);
            Clip.ScaleY = Maths.StepTo(Clip.ScaleY, TargetScale.Y, 0.0005f);
            Eye.Scale = 1f / Clip.ScaleX * eyeScale;
            float y = Clip.Position.Y - (bodyBackground.TextureSize.Y * (1f - Clip.ScaleY) / 2f) - 2f;
            Clip.Position = new Vector2(Clip.Position.X, y);
        }

        public void TryWakeUp()
        {
            if (Sleep && velocity > 0.1f)
            {
                eyeClosed = false;
                Sleep = false;
                Eye.AnimationEndEvent.RemoveListener(OnEyeClose);
                Eye.PlayAnimation(new EyeAnimation("McEyeOpen", null, lockY: true), force: true);
            }
        }

        public void TeleportTail(Vector2 position)
        {
            if (blackTails != null)
            {
                BlackTail blackTail = blackTails[0];
                blackTail.Body = null;
                blackTail.Target = position;
                Builder.ChangeChildLayer(blackTail, -3);
            }
        }

        public void NewBlackTail()
        {
            if (blackTails != null)
            {
                BlackTail blackTail = new(this);
                blackTails.Insert(0, blackTail);
                Builder.Add(blackTail, 3);
            }
        }

        public void TrySleep()
        {
            if (!Sleep && timeToSleep < 0f)
            {
                Eye.PlayAnimation(new EyeAnimation("McEyeCloseSlow", null, lockY: true), force: true);
                Sleep = true;
                Eye.AnimationEndEvent.AddListener(OnEyeClose);
            }
        }

        protected void FinishLevelSpeedEyeAnimation(Vector2 targetPosition, float finishSpeed, string eyeAnimation)
        {
            if (Removed)
            {
                return;
            }
            finishSet = true;
            this.finishSpeed = finishSpeed;
            finishPosition = targetPosition;
            Body.BodyType = 0;
            Body.LinearVelocity = new Vector2(0f, 0f);
            Body.SetSensor(value: true);
            if (eyeAnimation != null)
            {
                Eye.PlayAnimation(new EyeAnimation(eyeAnimation), force: true);
                Eye.AnimationEndEvent.AddListener(OnEyeCloseFinish);
            }
            if (!restarting)
            {
                if (LevelCompleted)
                {
                    Game.Finished = true;
                }
                Schedule(OnFinish, 0.5f);
            }
            FinishEvent.SendEvent();
            DestroyEvent.SendEvent();
            FadeOutTails();
        }

        protected virtual void FadeOutTails()
        {
            if (blackTails != null)
            {
                foreach (BlackTail blackTail in blackTails)
                {
                    _ = blackTail.FadeOut(0.3f);
                }
            }
            _ = (Tail?.ScaleTo(0.3f, 0f));
        }

        protected virtual void DoFinish()
        {
            if (LevelCompleted)
            {
                Game.Finish(Builder.ToPoint(finishPosition));
            }
            else
            {
                Schedule(CallFail, finishPause);
            }
        }

        protected virtual void FinishLevelSpeed(Vector2 targetPosition, float finishSpeed)
        {
            FinishLevelSpeedEyeAnimation(targetPosition, finishSpeed, "McEyeClose");
        }

        public void CompleteLevelSpeed(Vector2 targetPosition, float finishSpeed)
        {
            LevelCompleted = true;
            Game.Amie?.MarkLevelCompleted();
            FinishLevelSpeed(targetPosition, finishSpeed);
        }

        public void FailLevelSpeedPause(Vector2 targetPosition, float finishSpeed, float pause)
        {
            finishPause = pause;
            FinishLevelSpeed(targetPosition, finishSpeed);
        }

        public void FailLevelSpeedPauseEyeAnimation(Vector2 targetPosition, float finishSpeed, float pause, string eyeAnimation)
        {
            finishPause = pause;
            FinishLevelSpeedEyeAnimation(targetPosition, finishSpeed, eyeAnimation);
        }

        private void Hide()
        {
            Clip.Visible = false;
        }

        public static void HideEye()
        {
        }

        private void OnFinish()
        {
            if (!restarting)
            {
                Finished = true;
                if (!Removed)
                {
                    DoFinish();
                }
            }
        }

        private void CallFail()
        {
            eating = false;
            if (!restartOnEating)
            {
                Game.Fail(0f);
            }
            restartOnEating = false;
        }

        private void OnEyeCloseFinish()
        {
            Eye.AnimationEndEvent.RemoveListener(OnEyeCloseFinish);
            Eye.SetEyeContent(new EyeAnimation("McEyeClose"));
            MovieClip movieClip = (MovieClip)Eye.CurrentBackground;
            movieClip.GotoAndStop(movieClip.TotalFrames - 1);
            EyeAnimationsAllowed = false;
            Eye.Visible = false;
        }

        private void OnEyeClose()
        {
            if (Sleep)
            {
                eyeClosed = true;
                Eye.PlayAnimation(new EyeAnimation("McEyeSleep", null, lockY: true), force: true);
            }
        }

        public void OnHitEnd()
        {
            Eye.AnimationEndEvent.RemoveListener(OnHitEnd);
            Eye.PlayAnimation(new EyeAnimation("McEyeBlink"), force: false);
        }

        public override void UpdateRotation()
        {
            bodyBackground.RotationRadians = Body.Rotation;
        }
    }
}
