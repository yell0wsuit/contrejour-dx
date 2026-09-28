using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class SpringBodyClip : ContreJourBodyClip, IClickable, IRestartable
{
    private const float SLOW_SMOKE_MULT = 6f;

    private const float MAX_JOINED_DISTANCE = 1.3333334f;

    private const float TO_CENTER_FORCE = 7f;

    private const float SCALE_STEP = 7f;

    private const float OPACITY_STEP = 200f;

    private const float SMOKE_RANGE = 20f;

    private const float MAX_TIME_TO_SMOKE = 2.3f;

    private const float MIN_TIME_TO_SMOKE = 1.5f;

    private const float LAUNCH_PAUSE = 0.2f;

    private const int SPIT_FRAMES = 5;

    private const int BASE_FRAME = 7;

    private const int SMOKE_PARTS = 10;

    private const float BUBBLE_DISTANCE = 2.6666667f;

    private const float JOINT_STEP = 2f / 3f;

    private const float MAX_DISTANCE = 1.1666666f;

    private const float IMPULSE = 25f;

    private const float IMPULSE_DISTANCE = 6.6666665f;

    private const float SUCK_DISTANCE = 150f;

    private const int SUCK_FORCE = 100;

    private static readonly Vector2 BODY_CENTER = new Vector2(0f, 20f);

    private static readonly Vector2 SMOKE_POINT = new Vector2(0f, 60f);

    private static readonly Vector2 SUCK_POINT = new Vector2(0f, 40f);

    private static readonly Vector2 STICKY_POINT = new Vector2(0f, 40f);

    protected Vector2 bodyCenterVec;

    protected CosChanger breatheChanger;

    protected float launchTime;

    protected MovieClip movie;

    protected Node redDot;

    protected Vector2 relativeStickedPosition;

    protected Vector2 size;

    protected WhiteSmoke smoke;

    protected float startScale;

    protected ILaunchable sticked;

    protected float suckDistance;

    protected Vector2 suckPoint;

    protected float timeToSmoke;

    protected virtual Vector2 SmokePoint => SMOKE_POINT;

    public Vector2 WorldSuckPoint => VectorUtil.Rotate(suckPoint, base.BodyAngle) + Body.Position;

    public bool DisableHeroFocus => true;

    public SpringBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        if (!base.Game.BlackSide)
        {
            _clip = _builder.ReplaceClipWith(_clip, GetClipName());
            clip = _clip;
        }
        clip.Parent.ChangeChildLayer(_clip, 2);
        Body.IsBullet = true;
        config["noShadow"] = "true";
        config["noSound"] = "true";
        movie = (MovieClip)clip;
        movie.Stoped = true;
        movie.Repeat = false;
        movie.MinFrame = 7f;
        CreateShadow();
        startScale = clip.ScaleX;
        suckPoint = builder.ToVec(SUCK_POINT * clip.ScaleX);
        suckDistance = 150f * clip.ScaleX * builder.SizeMult;
        bodyCenterVec = VectorUtil.Rotate(builder.ToVec(BODY_CENTER * clip.ScaleX), base.InitialBodyAngle);
        breatheChanger = new CosChanger(0.06f, 0.07f);
        breatheChanger.MinValue = 0.95f;
        breatheChanger.MaxValue = 1.04f;
        foreach (Fixture fixture in Body.FixtureList)
        {
            fixture.Friction = 1f;
            if (IsSticky(fixture))
            {
                fixture.IsSensor = true;
            }
        }
        launchTime = -0.2f;
        smoke = new WhiteSmoke(base.Game.BlackSide ? "common/McWhiteSmokeBlack" : "common/McWhiteSmoke");
        if (base.Game.BonusChapter)
        {
            smoke.Color = ContreJourConstants.GreenLightColor;
        }
        smoke.ScaleDownOnDestroy = false;
        RefreshSmokeAngle();
        builder.Add(smoke, 11);
        CreateSmoke();
        timeToSmoke = Maths.Random(1.5f, 2.3f);
        clip.AddedToStageEvent += RefreshPoints;
        SetThinSmokeRange();
    }

    public bool UseForZoom()
    {
        return false;
    }

    public virtual int Priority(Vector2 touchPoint)
    {
        if (sticked == null || !IsTouchDistance(touchPoint))
        {
            return -10;
        }
        return 2;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public virtual bool TouchBegan(Touch touch)
    {
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        //IL_002d: Invalid comparison between Unknown and I4
        if (IsTouchDistance(builder.TouchRootVec(touch)))
        {
            if (sticked != null && (int)sticked.Body.BodyType == 2)
            {
                return false;
            }
            LaunchTouching();
            if (sticked != null)
            {
                Launch();
            }
            else if (Maths.FuzzyEquals(movie.CurrentFrame, 7f) && movie.Stoped)
            {
                SoundManager.PlaySound("perdelkaOutEmpty1", 0.7f);
                Spit();
            }
            return true;
        }
        return false;
    }

    public virtual bool TouchMove(Touch touch)
    {
        return true;
    }

    public virtual void TouchOut(Touch touch)
    {
    }

    public virtual void TouchEnd(Touch touch)
    {
    }

    public virtual void Restart()
    {
        launchTime = -0.2f;
    }

    protected virtual string GetClipName()
    {
        return base.Game.Choose("McSpringView_5", null, "McSpringViewWhite", null, "McSpringView_6");
    }

    private EventSender GetDestroyEvent(Body teleportBody)
    {
        return ((ISnotLinked)teleportBody.UserData).DestroyEvent;
    }

    protected virtual void SetSticked(ILaunchable value)
    {
        if (sticked != null)
        {
            sticked.DestroyEvent.RemoveListener(OnTeleport);
        }
        sticked = value;
        if (sticked != null)
        {
            sticked.DestroyEvent.AddListener(OnTeleport);
        }
    }

    public void RefreshSmokeAngle()
    {
        smoke.Angle = new RandomRange(clip.RotationDegrees + 90f, 20f);
    }

    protected virtual void CreateShadow()
    {
        Node node = new Sprite(base.Game.ChooseSide("common/McSpringShadow", "chapter4/McSpringShadowWhite", "common/McSpringShadow_5"));
        builder.AddChildBefore(node, clip);
        node.Position = clip.Position;
        node.RotationRadians = clip.RotationRadians;
        node.ScaleX = clip.ScaleX;
        node.ScaleY = clip.ScaleY;
    }

    protected virtual void RefreshPoints()
    {
        smoke.SmokePosition = clip.LocalToNode(SmokePoint, builder.GameRoot);
    }

    private float OpacityStep()
    {
        return 200f;
    }

    public void SetThinSmokeRange()
    {
        SetSmokeRange(0f);
        smoke.MaxOpacity = 75f;
        smoke.OpacityStep = OpacityStep() / 6f;
        smoke.ScaleStep = 1.1666666f;
    }

    public void SetWhideSmokeRange()
    {
        SetSmokeRange(20f);
        smoke.MaxOpacity = 120f;
        smoke.OpacityStep = OpacityStep();
        smoke.ScaleStep = 7f;
    }

    public void SetSmokeRange(float range)
    {
        smoke.HorizontalPosition = new RandomRange(smoke.SmokePosition.X, range);
        smoke.VerticalPosition = new RandomRange(smoke.SmokePosition.Y, range);
    }

    public void CreateSmoke()
    {
        for (int i = 0; i < 10; i++)
        {
            smoke.AddParticle(clip.Position).Visible = false;
        }
    }

    private void ShowSmoke()
    {
        smoke.ShowAllParticles();
    }

    public bool CanLaunch(object bodyClip)
    {
        if (bodyClip is ILaunchable)
        {
            return ((ILaunchable)bodyClip).CanLaunch();
        }
        return false;
    }

    private bool IsTouchDistance(Vector2 touchPosition)
    {
        return GetTouchDistance(touchPosition) < 1.1666666f;
    }

    private float GetTouchDistance(Vector2 touchPosition)
    {
        return Body.GetWorldPoint(bodyCenterVec).DistanceTo(touchPosition);
    }

    public void LaunchTouching()
    {
        for (ContactEdge val = Body.ContactList; val != null; val = val.Next)
        {
            if ((!val.Contact.FixtureA.IsSensor || !val.Contact.FixtureB.IsSensor) && (IsSticky(val.Contact.FixtureA) || IsSticky(val.Contact.FixtureB)) && val.Contact.IsTouching)
            {
                object userData = val.Other.UserData;
                if (userData != sticked && CanLaunch(userData))
                {
                    ApplyImpulseTo((ILaunchable)userData);
                }
            }
        }
    }

    public void Launch()
    {
        sticked.SetSpeedLocked(value: false);
        sticked.HitEnabled = true;
        base.Game.FocusOnHero();
        sticked.Body.BodyType = (BodyType)2;
        ApplyImpulseTo(sticked);
        launchTime = base.Game.TotalTime;
        SetSticked(null);
        SoundManager.PlaySound("perdelkaOut0", 0.7f);
        Spit();
        UserData.Instance.SpringShot++;
    }

    public void ApplyImpulseTo(ILaunchable bodyClip)
    {
        //IL_000c: Unknown result type (might be due to invalid IL or missing references)
        //IL_0012: Invalid comparison between Unknown and I4
        float num = 1f;
        if ((int)bodyClip.Body.BodyType == 2)
        {
            float num2 = WorldSuckPoint.DistanceTo(bodyClip.Body.Position) - bodyClip.Radius();
            if (num2 > 0f)
            {
                num = Math.Max(0f, (6.6666665f - num2) / 6.6666665f);
            }
        }
        ApplyImpulseTo(25f * num, bodyClip.Body);
    }

    public void ApplyImpulseTo(float impulse, Body _body)
    {
        impulse = impulse * _body.Mass * startScale;
        _body.LinearVelocity = Vector2.Zero;
        _body.ApplyLinearImpulse(VectorUtil.ToVector(impulse, MathHelper.ToRadians(clip.RotationDegrees + 90f)), _body.WorldCenter);
    }

    public void Spit()
    {
        SetWhideSmokeRange();
        ShowSmoke();
        movie.Rewind = true;
        movie.MinFrame = 0f;
        movie.MaxFrame = movie.TotalFrames;
        movie.Stoped = false;
    }

    private void EnableJoin()
    {
    }

    public float JointStep()
    {
        return 2f / 3f;
    }

    protected virtual void UpdateSticked()
    {
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Invalid comparison between Unknown and I4
        if ((int)sticked.Body.BodyType == 2)
        {
            Vector2 worldSuckPoint = WorldSuckPoint;
            Vector2 vector = worldSuckPoint - sticked.Body.Position;
            float num = vector.Length();
            vector.Normalize();
            vector *= sticked.Body.Mass;
            vector *= Math.Min((suckDistance - num) * 100f, 200f);
            sticked.Body.ApplyForce(vector, sticked.Body.WorldCenter);
            Vector2 vector2 = VectorUtil.Rotate(new Vector2(1f, 0f), base.BodyAngle);
            float num2 = VectorUtil.Projection(sticked.Body.LinearVelocity, vector2);
            float num3 = VectorUtil.Projection(worldSuckPoint - sticked.Body.Position, vector2);
            if (num2 * num3 < 0f || (Math.Abs(num2) < 10f && Math.Abs(num3) > 1f))
            {
                Vector2 vector3 = vector2;
                vector3 *= num3;
                vector3 *= sticked.Body.Mass;
                vector3 *= 200f;
                sticked.Body.ApplyForce(vector3, sticked.Body.WorldCenter);
            }
            relativeStickedPosition = sticked.Body.Position - Body.Position;
            relativeStickedPosition = VectorUtil.Rotate(relativeStickedPosition, 0f - base.BodyAngle);
            FixClosePosition(relativeStickedPosition);
            CheckFixed(num3);
        }
    }

    private void ApplyRelativePosition(Vector2 relativePosition)
    {
        Vector2 vector = VectorUtil.Rotate(relativePosition, base.BodyAngle);
        vector += Body.Position;
        sticked.Body.SetTransform(vector, sticked.Body.Rotation);
    }

    public void FixClosePosition(Vector2 relativePosition)
    {
        float num = relativePosition.Y - 2.2f;
        if (num > 0f && Math.Abs(relativePosition.X) * 2f > num)
        {
            relativePosition.X = Maths.StepTo(relativePosition.X, (float)Math.Sign(relativePosition.X) * num / 2f, 0.3f);
            ApplyRelativePosition(relativePosition);
        }
        else if (num < 0f)
        {
            relativePosition.X = Maths.StepTo(relativePosition.X, 0f, 0.3f);
            ApplyRelativePosition(relativePosition);
        }
    }

    public void CheckFixed(float positionProjection)
    {
        if (Math.Abs(positionProjection) < 0.1f && CheckStickedBodyContacts("base"))
        {
            sticked.Body.BodyType = (BodyType)1;
            sticked.Body.LinearVelocity = Vector2.Zero;
            sticked.Body.AngularVelocity = 0f;
        }
    }

    public override void Update(float time)
    {
        //IL_001a: Unknown result type (might be due to invalid IL or missing references)
        //IL_0020: Invalid comparison between Unknown and I4
        base.Update(time);
        if (sticked != null)
        {
            if ((int)sticked.Body.BodyType == 1 || CheckBodyContactsKeyCount(sticked.Body, "sticky", 1))
            {
                UpdateSticked();
                UpdateSuckingClip();
            }
            else
            {
                FixAnimation();
                SetSticked(null);
            }
        }
        else if (movie.Stoped && Maths.FuzzyEquals(movie.CurrentFrame, 7f))
        {
            breatheChanger.Update(time);
            clip.ScaleY = breatheChanger.Value * startScale;
            clip.ScaleX = (2f - breatheChanger.Value) * startScale;
            if (timeToSmoke < 0f)
            {
                timeToSmoke = Maths.Random(1.5f, 2.3f);
                int num = 0;
                while (smoke.Particles.Count < 10 && num < 3)
                {
                    SetThinSmokeRange();
                    ((GravityParticle)smoke.AddOrGetInvisible()).Speed *= 1f / 6f;
                    num++;
                }
            }
        }
        if (Maths.FuzzyEquals(movie.CurrentFrame, 0f))
        {
            movie.CurrentFrame = 7f;
            movie.Stoped = true;
        }
        timeToSmoke -= time;
    }

    private void ReverseSmoke()
    {
        smoke.ScaleStep = -10.5f;
        foreach (GravityParticle particle in smoke.Particles)
        {
            Vector2 speed = particle.Speed * -6f;
            float num = speed.Length();
            if (num > 300f)
            {
                speed *= 300f / num;
            }
            particle.Speed = speed;
        }
    }

    public void UpdateSuckingClip()
    {
        //IL_000b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0011: Invalid comparison between Unknown and I4
        float num = (((int)sticked.Body.BodyType == 2) ? Math.Min(1f, relativeStickedPosition.Y / suckDistance) : 0f);
        movie.CurrentFrame = 7f + Math.Max(5f * (1f - num) - 1f, 0f);
        if (Maths.FuzzyEquals(num, 0f))
        {
            movie.Stoped = true;
        }
    }

    private void ProcessCollisionPoint(Body body2, Contact point)
    {
        if (sticked == null && !(base.Game.TotalTime - launchTime < 0.2f))
        {
            object userData = body2.UserData;
            if (point.IsTouching && (!point.FixtureA.IsSensor || !point.FixtureB.IsSensor) && CanLaunch(userData) && (IsSticky(point.FixtureA) || IsSticky(point.FixtureB) || CheckBodyContactsKeyCount(body2, "base", 2)))
            {
                ReverseSmoke();
                ILaunchable launchable = (ILaunchable)userData;
                SetSticked(launchable);
                launchable.HitEnabled = false;
                SoundManager.PlaySound("perdelkaOut0", 0.7f);
            }
        }
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
        ProcessCollisionPoint(body2, point);
    }

    public override void OnCollisionPoint(Body body2, Contact point)
    {
        ProcessCollisionPoint(body2, point);
    }

    private void OnTeleport()
    {
        //IL_0013: Unknown result type (might be due to invalid IL or missing references)
        //IL_0019: Invalid comparison between Unknown and I4
        if (sticked != null)
        {
            if ((int)sticked.Body.BodyType == 1)
            {
                sticked.Body.BodyType = (BodyType)2;
            }
            Spit();
            SetSticked(null);
        }
    }

    public override void OnCollisionEndPoint(Body body2, Contact point)
    {
        if (sticked != null && body2 == sticked.Body && !Body.IsTouching(sticked.Body))
        {
            FixAnimation();
            SetSticked(null);
        }
    }

    public void FixAnimation()
    {
        if (Maths.FuzzyNotEquals(movie.CurrentFrame, 7f))
        {
            movie.MinFrame = 7f;
            movie.Rewind = true;
            movie.Stoped = false;
        }
    }

    public bool CheckBodyContactsKeyCount(Body target, string fixtureKey, int count)
    {
        ContactEdge val = target.ContactList;
        int num = 0;
        while (val != null)
        {
            if (val.Contact.IsTouching && (CheckFixtureKey(val.Contact.FixtureA, fixtureKey) || CheckFixtureKey(val.Contact.FixtureB, fixtureKey)))
            {
                num++;
            }
            if (num == count)
            {
                return true;
            }
            val = val.Next;
        }
        return false;
    }

    public bool CheckStickedBodyContacts(string fixtureKey)
    {
        return CheckBodyContactsKeyCount(sticked.Body, fixtureKey, 2);
    }

    public bool CheckFixtureKey(Fixture fixture, string key)
    {
        if (!(fixture.UserData is Hashtable hashtable))
        {
            return false;
        }
        return hashtable.ContainsKey(key);
    }

    public bool IsSticky(Fixture fixture)
    {
        return CheckFixtureKey(fixture, "sticky");
    }
}
