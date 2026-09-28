using System;

using ContreJour.Clips.chapter5;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatableSpringBodyClip : RotatableSpringBase, IRestartable
{
    private static readonly float TouchRadius = 80f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private static readonly float TouchDistanceLimit = 40f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private Touch rotateTouch;

    private float touchPointAngle;

    private float touchPointSpeed;

    private readonly float touchPointNeededSpeed;

    private float lastDirection = 1f;

    private float startAngle;

    private float startTouchAngle;

    private Sprite circle;

    private Sprite touchPoint;

    private float targetRotation;

    private float touchAngle;

    private readonly float minOpacity = 0.5f;

    private readonly float startSpringWidth;

    private readonly Trajectory trajectory;

    private readonly ContreJourGame game;

    protected override Vector2 SmokePoint => new(0f, 40f);

    protected override bool IsMoving => rotateTouch != null;

    public RotatableSpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        game = (ContreJourGame)this.builder.Game;
        Body.BodyType = (BodyType)1;
        bodyCenterVec = Vector2.Zero;
        touchPoint = new McRotatorPoint();
        circle = new McRotatorCircle
        {
            Scale = 1.6f
        };
        if (Game.BonusChapter)
        {
            circle.Color = ContreJourConstants.GreenLightColor;
            touchPoint.Color = ContreJourConstants.GreenLightColor;
            minOpacity = 0.9f;
        }
        this.builder.AddChildAfter(circle, this.clip);
        circle.AddChild(touchPoint);
        touchPoint.IgnoreParentOpacity = true;
        circle.Position = this.clip.Position;
        touchPoint.Position = new Vector2(50f, 0f);
        touchPointSpeed = Maths.Random(0.02f, 0.03f) / 1.5f;
        touchPointNeededSpeed = touchPointSpeed;
        startSpringWidth = config.GetFloat("Width");
        trajectory = new Trajectory(game)
        {
            Impulse = startSpringWidth / 32f
        };
        this.builder.AddChildAfter(trajectory, circle);
        foreach (Fixture fixture in Body.FixtureList)
        {
            if (fixture.UserData is Hashtable && ((Hashtable)fixture.UserData).GetString("id", null) == "touch")
            {
                fixture.IsSensor = true;
                break;
            }
        }
        StopActions();
    }

    protected override string GetClipName()
    {
        return !Game.BonusChapter ? "McRotatableSpring" : "McRotatableSpring_6";
    }

    public override void Restart()
    {
        base.Restart();
        targetRotation = 0f.SimplifyAngle((float)((double)Body.Rotation - Math.PI));
    }

    public override int Priority(Vector2 touchPosition)
    {
        return touchPosition.DistanceTo(Body.Position) < TouchDistanceLimit ? base.Priority(touchPosition) : IsRotatorTouched(touchPosition) ? 0 : -100;
    }

    public override float TouchDistance(Vector2 touchPosition)
    {
        float num = touchPosition.DistanceTo(Body.Position);
        return Math.Min(num, Math.Abs(num - TouchRadius));
    }

    private void UpdateTouchPoint()
    {
        if (rotateTouch != null)
        {
            float num = touchAngle.SimplifyAngle(touchPointAngle - (float)Math.PI);
            touchPointSpeed = Maths.StepTo(touchPointSpeed, Math.Abs(num - touchPointAngle) / 10f, 0.01f);
            touchPointAngle = Maths.StepTo(touchPointAngle, num, touchPointSpeed);
        }
        else
        {
            touchPointSpeed = Maths.StepTo(touchPointSpeed, touchPointNeededSpeed, 0.002f);
            touchPointAngle += touchPointSpeed * Math.Sign(lastDirection);
        }
        touchPoint.Position = VectorUtil.ToVector(50f, touchPointAngle);
        touchPoint.RotationRadians = touchPointAngle;
    }

    protected override void SetSticked(ILaunchable value)
    {
        trajectory.FadeOutDelay = (value != null) ? 2 : 0;
        base.SetSticked(value);
        if (value == null)
        {
            trajectory.Enabled = true;
            trajectory.Enabled = false;
        }
    }

    public override void Update(float time)
    {
        if (rotateTouch != null)
        {
            float num = touchAngle;
            touchAngle = GetTouchAngle();
            if (num != touchAngle)
            {
                lastDirection = Math.Sign(touchAngle - num);
            }
            targetRotation = startAngle + touchAngle - startTouchAngle;
            targetRotation = Maths.Round(targetRotation, (float)Math.PI / 8f);
            targetRotation = targetRotation.SimplifyAngle((float)((double)Body.Rotation - Math.PI));
            trajectory.Enabled = sticked != null;
        }
        else
        {
            trajectory.Enabled = false;
        }
        if (Maths.FuzzyNotEquals(Body.Rotation, targetRotation))
        {
            int num2 = ((double)Math.Abs(targetRotation - Body.Rotation) > Math.PI / 4.0) ? 5 : 10;
            Body.AngularVelocity = (targetRotation - Body.Rotation) / time / num2;
        }
        else
        {
            Body.AngularVelocity = 0f;
            Body.Rotation = targetRotation;
            RefreshSmokeAngle();
        }
        trajectory.Angle = Body.Rotation + ((float)Math.PI / 2f);
        trajectory.Position = Position + VectorUtil.ToVector(80f, trajectory.Angle);
        UpdateTouchPoint();
        base.Update(time);
    }

    public override bool TouchBegan(Touch touch)
    {
        if (!base.TouchBegan(touch) && IsRotatorTouched(builder.TouchRootVec(touch)))
        {
            RunActions();
            rotateTouch = touch;
            startAngle = Body.Rotation;
            startTouchAngle = GetTouchAngle();
            return true;
        }
        return true;
    }

    private bool IsRotatorTouched(Vector2 touchPosition)
    {
        float num = Body.Position.DistanceTo(touchPosition);
        return rotateTouch == null && Math.Abs(num - TouchRadius) < TouchDistanceLimit;
    }

    private float GetTouchAngle()
    {
        return (builder.TouchRootVec(rotateTouch) - Body.Position).Atan2();
    }

    public override void TouchEnd(Touch touch)
    {
        if (touch == rotateTouch)
        {
            trajectory.Enabled = false;
            rotateTouch = null;
            StopActions();
        }
        else
        {
            base.TouchEnd(touch);
        }
    }

    private void StopActions()
    {
        circle.Tweener.Stop();
        _ = circle.FadeTo(0.5f, minOpacity);
        touchPoint.Tweener.Stop();
        _ = touchPoint.FadeTo(0.3f, minOpacity);
    }

    private static void RunFadeInOut(Node node, float _in, float _out)
    {
        node.Tweener.Stop();
        _ = node.Tweener.RepeatSequenceForever(2.5f).FadeTo(_out).Next(2.5f)
            .FadeTo(_in);
    }

    private void RunActions()
    {
        RunFadeInOut(circle, 1f, 0.7f);
        touchPoint.Tweener.Stop();
        _ = touchPoint.FadeIn(0.3f);
    }
}
