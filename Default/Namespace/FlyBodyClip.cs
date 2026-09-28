using System;

using ContreJour.Clips.chapter5;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class FlyBodyClip : ContreJourBodyClip, IClickable
{
    protected FlyEye eye;

    protected Sprite bodySprite;

    protected Vector2 initialPosition;

    protected bool freeFlight;

    protected bool stoped;

    protected FlyWings leftWings;

    protected FlyWings rightWings;

    protected float backTime;

    protected float scaredTime;

    protected float heroScaredTime;

    private static readonly Vector2 WingsPosition = new(8f, 2f);

    public bool DisableHeroFocus => true;

    public FlyBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        bodySprite = new McFlyBody();
        clip = bodySprite;
        this.clip = clip;
        _ = builder.AddChild(clip);
        eye = new FlyEye(Game, visible: true, Body.Position)
        {
            Scale = 0.65f
        };
        this.clip.AddChild(eye);
        scaredTime = 0f;
        Schedule(StartFly, Maths.Random(7f, 11f));
        initialPosition = Body.Position;
        leftWings = new FlyWings();
        rightWings = new FlyWings();
        leftWings.Position = new Vector2(0f - WingsPosition.X, WingsPosition.Y);
        rightWings.Position = WingsPosition;
        rightWings.ScaleX = -1f;
        this.clip.AddChild(leftWings);
        this.clip.AddChild(rightWings);
        Body.GravityScale = 0f;
    }

    public int Priority(Vector2 touchPosition)
    {
        return 0;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return true;
    }

    public bool TouchBegan(Touch touch)
    {
        SoundManager.PlaySound("fly", Maths.Random(0.15f, 0.35f));
        eye.PlayAnimation(new EyeAnimation("McEyeBlinkMonster"), force: false);
        eye.RandomPositionProvider = Game.GetTouchProvider(touch);
        Scare(builder.TouchRootVec(touch));
        return false;
    }

    public void Scare(Vector2 scarePoint)
    {
        scaredTime = Game.TotalTime + 2f;
        if (!freeFlight)
        {
            StartFly();
        }
        else
        {
            FlyOutAngle(1f, VectorUtil.Atan2(scarePoint, Body.Position));
        }
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    public void TouchEnd(Touch touch)
    {
    }

    public void SetFlying(bool value)
    {
        float num = value ? 0f : MathHelper.ToRadians(30f);
        _ = leftWings.RotateTo(0.5f, num);
        _ = rightWings.RotateTo(0.5f, 0f - num);
        leftWings.SetFlying(value);
        rightWings.SetFlying(value);
    }

    public void StartFly()
    {
        bool flag = freeFlight;
        freeFlight = true;
        SetFlying(value: true);
        stoped = false;
        if (!flag)
        {
            SoundManager.PlaySound("fly", 0.3f);
            Body.LinearDamping = 0.7f;
            backTime = Game.TotalTime + Maths.Random(30f, 45f);
            Schedule(ToBackground, 0.3f);
        }
        Fly();
    }

    private void ToBackground()
    {
        clip.Parent.ChangeChildLayer(clip, -3);
    }

    private void ToForeground()
    {
        clip.Parent.ChangeChildLayer(clip, 0);
    }

    public void EndFly()
    {
        float angle = VectorUtil.Atan2(Body.Position, initialPosition);
        float impulse = 6f * Body.Position.DistanceTo(initialPosition) / 6.6666665f;
        Body.LinearVelocity = Vector2.Zero;
        DoFlyImpulse(angle, impulse);
        freeFlight = false;
        _ = clip.ScaleTo(1.2f, 1f);
        Schedule(StartFly, Maths.Random(10f, 20f));
        Schedule(ToForeground, 0.3f);
    }

    public void ScheduleFly()
    {
        Schedule(Fly, Maths.Random(7f, 11f));
    }

    public void Fly()
    {
        if (freeFlight)
        {
            if (Game.TotalTime < backTime || Body.Position.Y < initialPosition.Y)
            {
                FlyOut();
                ScheduleFly();
            }
            else
            {
                EndFly();
            }
        }
    }

    public void FlyOut()
    {
        FlyOut(1f);
    }

    public void FlyOut(float impulseMult)
    {
        FlyOutAngle(impulseMult, (!(Body.Position.DistanceTo(initialPosition) > 6.6666665f)) ? Maths.Random((float)Math.PI / 12f, (float)Math.PI * 11f / 12f) : (VectorUtil.Atan2(Body.Position, initialPosition) + Maths.Random(-(float)Math.PI / 6f, (float)Math.PI / 6f)));
    }

    public void FlyOutAngle(float impulseMult, float angle)
    {
        _ = TweeningExtensions.ScaleTo(scale: Maths.Random(0.6f, 0.95f), node: clip, time: 1.2f);
        DoFlyImpulse(angle, Maths.Random(3f, 6f) * impulseMult);
    }

    public void DoFlyImpulse(float angle, float impulse)
    {
        Vector2 vector = VectorUtil.ToVector(impulse * Body.Mass, angle);
        Body.ApplyLinearImpulse(vector, Body.WorldCenter);
    }

    public override void Update(float time)
    {
        base.Update(time);
        eye.UpdateNode(time);
        if (!freeFlight && !stoped)
        {
            TryStop();
        }
        eye.ProviderEnabled = true;
        if (freeFlight && Game.TotalTime >= scaredTime)
        {
            float num = Body.LinearVelocity.Length();
            if (num > 1f)
            {
                eye.ProviderEnabled = false;
                eye.ViewAngle = VectorUtil.Atan2(Body.LinearVelocity);
                eye.ViewDistance = num / 2f;
            }
        }
        if (freeFlight)
        {
            CheckOutOfBorder();
        }
        if (Body.Position.DistanceTo(Game.HeroPositionVec) < 1.6666666f && heroScaredTime < Game.TotalTime)
        {
            heroScaredTime = Game.TotalTime + 2f;
            Scare(Game.HeroPositionVec);
        }
    }

    public void CheckOutOfBorder()
    {
        if (clip.Position.X < 50f)
        {
            DoFlyImpulse(Maths.Random(0f, (float)Math.PI / 4f), Maths.Random(3f, 6f));
        }
        else if (clip.Position.X > Game.LevelSize.X - 50f)
        {
            DoFlyImpulse(Maths.Random((float)Math.PI * 3f / 4f, (float)Math.PI), Maths.Random(3f, 6f));
        }
        else if (clip.Position.Y > Game.LevelSize.Y - 50f)
        {
            DoFlyImpulse(Maths.Random((float)Math.PI * -3f / 4f, -(float)Math.PI / 4f), Maths.Random(3f, 6f));
        }
        else if (clip.Position.Y < 50f)
        {
            DoFlyImpulse(Maths.Random((float)Math.PI / 4f, (float)Math.PI * 3f / 4f), Maths.Random(3f, 6f));
        }
    }

    public override void UpdateRotation()
    {
        float target = -5f * Body.LinearVelocity.X;
        clip.RotationDegrees = Maths.StepTo(clip.RotationDegrees, target, 1f);
        bodySprite.RotationDegrees = clip.RotationDegrees;
    }

    public void TryStop()
    {
        if (!stoped && initialPosition.DistanceTo(Body.Position) <= 0.1f)
        {
            Body.LinearDamping = 10f;
            SetFlying(value: false);
            stoped = true;
        }
    }
}
