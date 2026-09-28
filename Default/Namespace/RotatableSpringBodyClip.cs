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
    private const float ACTION_TIME = 2.5f;

    private const float ANGLE_REMAINDER = (float)Math.PI / 8f;

    private const float TOUCH_RADIUS_PIXELS = 80f;

    private const int CIRCLE_RADIUS = 50;

    private const float DEFAULT_WIDTH = 32f;

    private static readonly float TOUCH_RADIUS = 80f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private static readonly float TOUCH_DISTANCE = 40f * Box2DConfig.DefaultConfig.SizeMultiplier;

    private Touch rotateTouch;

    private float touchPointAngle;

    private float touchPointSpeed;

    private float touchPointNeededSpeed;

    private float lastDirection = 1f;

    private float startAngle;

    private float startTouchAngle;

    protected Sprite circle;

    protected Sprite touchPoint;

    private float targetRotation;

    private float touchAngle;

    private float minOpacity = 0.5f;

    private float startSpringWidth;

    private Trajectory trajectory;

    private ContreJourGame game;

    protected override Vector2 SmokePoint => new Vector2(0f, 40f);

    protected override bool IsMoving => rotateTouch != null;

    public RotatableSpringBodyClip(LevelBuilderBase _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        game = (ContreJourGame)builder.Game;
        Body.BodyType = (BodyType)1;
        bodyCenterVec = Vector2.Zero;
        touchPoint = new McRotatorPoint();
        circle = new McRotatorCircle();
        circle.Scale = 1.6f;
        if (base.Game.BonusChapter)
        {
            circle.Color = ContreJourConstants.GreenLightColor;
            touchPoint.Color = ContreJourConstants.GreenLightColor;
            minOpacity = 0.9f;
        }
        builder.AddChildAfter(circle, clip);
        circle.AddChild(touchPoint);
        touchPoint.IgnoreParentOpacity = true;
        circle.Position = clip.Position;
        touchPoint.Position = new Vector2(50f, 0f);
        touchPointSpeed = Maths.Random(0.02f, 0.03f) / 1.5f;
        touchPointNeededSpeed = touchPointSpeed;
        startSpringWidth = _config.GetFloat("Width");
        trajectory = new Trajectory(game);
        trajectory.Impulse = startSpringWidth / 32f;
        builder.AddChildAfter(trajectory, circle);
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
        if (!base.Game.BonusChapter)
        {
            return "McRotatableSpring";
        }
        return "McRotatableSpring_6";
    }

    public override void Restart()
    {
        base.Restart();
        targetRotation = 0f.SimplifyAngle((float)((double)Body.Rotation - Math.PI));
    }

    public override int Priority(Vector2 touchPoint)
    {
        if (touchPoint.DistanceTo(Body.Position) < TOUCH_DISTANCE)
        {
            return base.Priority(touchPoint);
        }
        if (IsRotatorTouched(touchPoint))
        {
            return 0;
        }
        return -100;
    }

    public override float TouchDistance(Vector2 touchPosition)
    {
        float num = touchPosition.DistanceTo(Body.Position);
        return Math.Min(num, Math.Abs(num - TOUCH_RADIUS));
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
            touchPointAngle += touchPointSpeed * (float)Math.Sign(lastDirection);
        }
        touchPoint.Position = VectorUtil.ToVector(50f, touchPointAngle);
        touchPoint.RotationRadians = touchPointAngle;
    }

    protected override void SetSticked(ILaunchable value)
    {
        trajectory.FadeOutDelay = ((value != null) ? 2 : 0);
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
            int num2 = (((double)Math.Abs(targetRotation - Body.Rotation) > Math.PI / 4.0) ? 5 : 10);
            Body.AngularVelocity = (targetRotation - Body.Rotation) / time / (float)num2;
        }
        else
        {
            Body.AngularVelocity = 0f;
            Body.Rotation = targetRotation;
            RefreshSmokeAngle();
        }
        trajectory.Angle = Body.Rotation + (float)Math.PI / 2f;
        trajectory.Position = base.Position + VectorUtil.ToVector(80f, trajectory.Angle);
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
        if (rotateTouch == null)
        {
            return Math.Abs(num - TOUCH_RADIUS) < TOUCH_DISTANCE;
        }
        return false;
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
        circle.FadeTo(0.5f, minOpacity);
        touchPoint.Tweener.Stop();
        touchPoint.FadeTo(0.3f, minOpacity);
    }

    private void RunFadeInOut(Node node, float _in, float _out)
    {
        node.Tweener.Stop();
        node.Tweener.RepeatSequenceForever(2.5f).FadeTo(_out).Next(2.5f)
            .FadeTo(_in);
    }

    private void RunActions()
    {
        RunFadeInOut(circle, 1f, 0.7f);
        touchPoint.Tweener.Stop();
        touchPoint.FadeIn(0.3f);
    }
}
