using System;
using System.Collections.Generic;

using ContreJour.Clips.common;
using ContreJour.Content;

using ContreJourMono.ContreJour.Game.Eyes;
using ContreJourMono.ContreJour.Game.Hero;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class HeroBodyClip : ContreJourBodyClip, IVectorPositionProvider, IBonusAcceptable, ISnotLinked, IEatable, ISpikesDestroyable, ILaunchable, IRadius, IBodyClip, ITeleportable, IRestartable
{
    public readonly EventSender FinishEvent = new();

    private readonly float eyeScale;

    private readonly List<BlackTail> blackTails;

    private readonly Sprite bodyBackground;

    private float breatheScale;

    private float breatheScaleStep;

    private bool disablePositionUpdate;

    private bool eating;

    protected HeroEye eye;
    private bool eyeClosed;

    private float finishPause;

    private Vector2 finishPosition;

    private bool finishSet;

    private float finishSpeed;

    protected bool finished;

    private bool hasToYawn;
    private readonly McHotspotwhite hotspot;

    private Vector2 initialPosition;

    private float lastFootPosition;

    private float lastHitTime;

    private float lastOnGroundTime;

    protected bool levelCompleted;

    private bool migthyPosted;

    private bool onGround;

    private float onGroundTime;

    private bool onPlasticine;

    private readonly Portal portal;

    private Vector2 previousSpeed;
    private bool restartOnEating;

    private bool restarting;

    private readonly Sprite shadow;

    protected bool sleep;

    private float sleepSoundTime;
    private bool speedyPosted;

    protected HeroTail tail;

    protected Vector2 targetScale;
    private float timeToSleep;

    private float velocity;

    private Vector2 worldSize;

    public EventSender TeleportEvent => DestroyEvent;

    public bool Removed { get; set; }

    public HeroEye Eye => eye;

    public bool SpeedLocked { get; set; }

    protected virtual float FirstRespawnTime => 0.5f;

    public virtual bool EyeAnimationsAllowed
    {
        get => eye.AnimationsAllowed;
        set => eye.AnimationsAllowed = value;
    }

    public bool EyeMoveAllowed
    {
        get => eye.MoveAllowed;
        set => eye.MoveAllowed = value;
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
            bodyBackground = (Sprite)ClipTypesCache.CreateNewNode(Game.ChooseSide("McHeroBlackView", "McHeroWhiteView", "McHeroBackView", "McHeroBackView", "McHeroView_6"));
            LevelBuilderBase.ReplaceChildWith(clip, bodyBackground);
        }
        else
        {
            bodyBackground = (Sprite)clip;
        }
        Vector2 position = clip.Position;
        bodyBackground.Position = Vector2.Zero;
        clip = new Node();
        this.clip = clip;
        clip.Position = position;
        shadow = (Sprite)ClipTypesCache.CreateNewNode(Game.ChooseSide("McHeroShadow", "McHeroShadowWhite", "McHeroShadow", "McHeroShadow"));
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
            this.builder.AddChildBefore(portal, this.clip);
            portal.ItemsScale = 0f;
            portal.ScaleStep = 0.1f;
        }
        onGroundTime = 0f;
        this.config["hasDust"] = true;
        hotspot = new McHotspotwhite();
        clip.AddChild(hotspot);
        eye = new HeroEye(Game);
        eyeScale = eye.Scale;
        clip.AddChild(eye);
        if (Game.BlackSide || Game.BonusChapter)
        {
            blackTails = [];
            BlackTail blackTail = new(this);
            blackTails.Add(blackTail);
            this.builder.Add(blackTail, 3);
        }
        else
        {
            tail = new HeroTail(Game.WhiteSide ? ContreJourConstants.WhiteTailColor : Color.Black);
            clip.AddChild(tail, -1);
        }
        breatheScale = 0f;
        sleep = false;
        timeToSleep = 7f;
        eyeClosed = false;
        Game.RegisterHero(this);
        onGround = false;
        targetScale = new Vector2(1f, 1f);
        breatheScaleStep = 0f;
        DestroyEvent = new EventSender();
        ContreJourGame.AddShadowSource();
    }

    public Vector2 BonusTarget()
    {
        return clip.Position;
    }

    public void ApplyBonus()
    {
        eye.ApplyBonus();
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
        eye.MoveAllowed = !value;
    }

    public void Restart()
    {
        restarting = true;
        FinishEvent.SendEvent();
        restartOnEating = eating;
        DestroyEvent.SendEvent();
        FadeOutTails();
        _ = (tail?.FadeOutAndHide(0.1f));
        Schedule(HideBody, 0.1f);
    }

    public void Explode()
    {
        eating = true;
        UserData.Instance.Accupuncture++;
        SoundManager.PlaySound("deathBySpikes5", 0.7f);
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
            tail?.Visible = false;
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
        clip.ScaleY = clip.ScaleX;
        _ = clip.ScaleTo(time, scale);
    }

    private void HideBody()
    {
        _ = clip.FadeOut(0.2f);
        Schedule(Respawn, 0.2f);
    }

    public void InitializeBody()
    {
        Body.BodyType = 0;
        clip.Visible = false;
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
        eye.ViewAngle = value;
        eye.ViewDistance = 0.7f;
    }

    public void SetPosition(Vector2 position)
    {
        clip.Position = position;
        Body.SetTransform(builder.ToIPhoneVec(position), Body.Rotation);
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
        builder.ChangeChildLayer(clip, 10);
        EyeAnimationsAllowed = true;
        eye.Visible = true;
        eye.SetDefaultView();
        if (tail != null)
        {
            tail.Tweener.Stop();
            tail.Scale = 1f;
            tail.Visible = true;
            tail.OpacityFloat = 1f;
        }
        Body.SetSensor(value: false);
        sleep = false;
        finishSet = false;
        disablePositionUpdate = false;
        finished = false;
        InitializeBody();
        restarting = false;
        clip.OpacityByte = 255;
        Body.SetTransform(initialPosition, Body.Rotation);
        ForceClipPosition();
        portal.TargetScale = 1f;
        Schedule(ShowClip, 0.7f);
        SoundManager.PlaySound("begin5", 0.5f);
        NewBlackTail();
    }

    private void ShowClip()
    {
        clip.Tweener.Stop();
        clip.OpacityFloat = 1f;
        clip.Visible = true;
        clip.Scale = 0f;
        _ = clip.ScaleTo(0.2f, 1f);
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
                    builder.RemoveChild(blackTail);
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
        eye.SetVelocity(Body.LinearVelocity);
        eye.UpdateNode(time);
        if (!finished && tail != null)
        {
            tail.LimitAngles = onGround;
            tail.SetMovementDirection(num2);
            tail.Speed = num3;
            tail.UpdateSpeed = sleep ? 0.2f : 1f;
        }
        previousSpeed = Body.LinearVelocity;
        velocity = num3;
        TryWakeUp();
        if ((int)Body.BodyType == 2 && Math.Abs(clip.ScaleY - 1f) < 0.04f && Body.JointList == null)
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
        float num = Body.LinearVelocity.DistanceTo(previousSpeed);
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
            eye.PlayAnimation(new EyeAnimation(null, "McEyeBallHit", lockY: true, lockX: true), force: false);
            eye.AnimationEndEvent.AddListener(OnHitEnd);
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
                num += (worldPoint.X - Body.Position.X) / builder.EngineConfig.SizeMultiplier;
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
            if (sleep && eyeClosed)
            {
                breatheScaleStep += 0.02f;
                breatheScaleStep = breatheScaleStep.SimplifyAngle(0f);
                float num = Maths.Cos(breatheScaleStep);
                breatheScale = num * 0.04f;
                targetScale.X = 1f + breatheScale;
                targetScale.Y = 1f + (breatheScale / 2f);
                MovieClip movieClip = (MovieClip)eye.CurrentBackground;
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
            targetScale.X = targetScale.Y = 1f;
            timeToSleep = 7f;
        }
        clip.ScaleX = Maths.StepTo(clip.ScaleX, targetScale.X, 0.0005f);
        clip.ScaleY = Maths.StepTo(clip.ScaleY, targetScale.Y, 0.0005f);
        eye.Scale = 1f / clip.ScaleX * eyeScale;
        float y = clip.Position.Y - (bodyBackground.TextureSize.Y * (1f - clip.ScaleY) / 2f) - 2f;
        clip.Position = new Vector2(clip.Position.X, y);
    }

    public void TryWakeUp()
    {
        if (sleep && velocity > 0.1f)
        {
            eyeClosed = false;
            sleep = false;
            eye.AnimationEndEvent.RemoveListener(OnEyeClose);
            eye.PlayAnimation(new EyeAnimation("McEyeOpen", null, lockY: true), force: true);
        }
    }

    public void TeleportTail(Vector2 position)
    {
        if (blackTails != null)
        {
            BlackTail blackTail = blackTails[0];
            blackTail.Body = null;
            blackTail.Target = position;
            builder.ChangeChildLayer(blackTail, -3);
        }
    }

    public void NewBlackTail()
    {
        if (blackTails != null)
        {
            BlackTail blackTail = new(this);
            blackTails.Insert(0, blackTail);
            builder.Add(blackTail, 3);
        }
    }

    public void TrySleep()
    {
        if (!sleep && timeToSleep < 0f)
        {
            eye.PlayAnimation(new EyeAnimation("McEyeCloseSlow", null, lockY: true), force: true);
            sleep = true;
            eye.AnimationEndEvent.AddListener(OnEyeClose);
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
            eye.PlayAnimation(new EyeAnimation(eyeAnimation), force: true);
            eye.AnimationEndEvent.AddListener(OnEyeCloseFinish);
        }
        if (!restarting)
        {
            if (levelCompleted)
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
        _ = (tail?.ScaleTo(0.3f, 0f));
    }

    protected virtual void DoFinish()
    {
        if (levelCompleted)
        {
            Game.Finish(builder.ToPoint(finishPosition));
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
        levelCompleted = true;
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
        clip.Visible = false;
    }

    public static void HideEye()
    {
    }

    private void OnFinish()
    {
        if (!restarting)
        {
            finished = true;
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
        eye.AnimationEndEvent.RemoveListener(OnEyeCloseFinish);
        eye.SetEyeContent(new EyeAnimation("McEyeClose"));
        MovieClip movieClip = (MovieClip)eye.CurrentBackground;
        movieClip.GotoAndStop(movieClip.TotalFrames - 1);
        EyeAnimationsAllowed = false;
        eye.Visible = false;
    }

    private void OnEyeClose()
    {
        if (sleep)
        {
            eyeClosed = true;
            eye.PlayAnimation(new EyeAnimation("McEyeSleep", null, lockY: true), force: true);
        }
    }

    public void OnHitEnd()
    {
        eye.AnimationEndEvent.RemoveListener(OnHitEnd);
        eye.PlayAnimation(new EyeAnimation("McEyeBlink"), force: false);
    }

    public override void UpdateRotation()
    {
        bodyBackground.RotationRadians = Body.Rotation;
    }
}
