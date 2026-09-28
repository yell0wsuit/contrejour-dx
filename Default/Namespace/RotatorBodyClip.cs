using System;

using ContreJour.Clips.chapter5;

using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class RotatorBodyClip : FurBodyClip, IClickable, IRestartable, ISnotHolder
{
    protected float targetAngle;

    protected Touch touch;

    protected float startTouchAngle;

    protected Sprite circle;

    protected Sprite touchPoint;

    protected bool touching;

    protected float lastDirection;

    protected float touchPointAngle;

    protected float touchPointSpeed;

    protected float touchPointNeededSpeed;

    protected float lastPointSpeed;

    private static readonly string[] GRASS_SPRITES = ["McRotatorGrass0", "McRotatorGrass1", "McRotatorGrass2"];

    private readonly float ACTION_TIME = 2.5f;

    private readonly int GRASS_COUNT = 26;

    private readonly float MAX_TOUCH_RADIUS = 3f;

    private readonly float MIN_TOUCH_RADIUS = 1f / 3f;

    private readonly float ANGLE_REMAINDER = (float)Math.PI / 8f;

    private readonly float WIDTH = 120f;

    private readonly float POINT_OFFSET = 48f;

    private readonly int GRASS_RADIUS = 60;

    public bool Rotating => Body.AngularVelocity != 0f;

    public bool DisableHeroFocus => false;

    public Vector2 SnotPosition => Body.Position;

    public RotatorBodyClip(ContreJourLevelBuilder _builder, object _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        lastDirection = 1f;
        Body.BodyType = (BodyType)1;
        circle = new McRotatorCircle();
        clip.AddChild(circle);
        touchPoint = new McRotatorPoint();
        clip.AddChild(touchPoint);
        touchPointSpeed = Maths.Random(0.02f, 0.03f);
        touchPointNeededSpeed = touchPointSpeed;
        RunActions();
    }

    public void Restart()
    {
        targetAngle = Maths.Round(targetAngle, (float)Math.PI * 2f);
    }

    public int Priority(Vector2 touchPosition)
    {
        return -1;
    }

    private void StopActions()
    {
        circle.Tweener.Stop();
        _ = circle.FadeTo(0.5f, 40f / 51f);
        touchPoint.Tweener.Stop();
        _ = touchPoint.FadeTo(0.5f, 40f / 51f);
    }

    private void RunFadeIn_Out_(Node node, float _in, float _out)
    {
        _ = node.Tweener.RepeatSequenceForever(ACTION_TIME).FadeTo(_out).Next(ACTION_TIME)
            .FadeTo(_in);
    }

    public void RunActions()
    {
        RunFadeIn_Out_(circle, 40f / 51f, 16f / 51f);
        RunFadeIn_Out_(touchPoint, 0.5882353f, 0.11764706f);
    }

    public override string GrassTexture()
    {
        return "chapter5/McRotatorGrassAll";
    }

    public override int GrassCount()
    {
        return GRASS_COUNT;
    }

    public override int GrassRadius()
    {
        return GRASS_RADIUS;
    }

    public override float Width()
    {
        return WIDTH;
    }

    public bool AcceptFreeTouches()
    {
        return false;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public override void Update(float time)
    {
        if (touch != null)
        {
            touchPointAngle = touchPointAngle.SimplifyAngle(startTouchAngle + targetAngle - (float)Math.PI);
            touchPointSpeed = Maths.StepTo(touchPointSpeed, 0.2f, 0.01f);
            float num = touchPointAngle;
            touchPointAngle = Maths.StepTo(touchPointAngle, targetAngle + startTouchAngle, touchPointSpeed);
            lastPointSpeed = Math.Abs(touchPointAngle - num);
        }
        else
        {
            touchPointSpeed = Maths.StepTo(touchPointSpeed, touchPointNeededSpeed, 0.003f);
            touchPointAngle += touchPointSpeed * Math.Sign(lastDirection);
        }
        touchPoint.Position = VectorUtil.ToVector(POINT_OFFSET, touchPointAngle - Body.Rotation);
        float num2 = targetAngle - Body.Rotation;
        if (Math.Abs(num2) > 0.0062831854f)
        {
            int num3 = (touch != null) ? 20 : 5;
            Body.AngularVelocity = num2 * num3;
        }
        else
        {
            Body.SetTransform(Body.Position, targetAngle);
            Body.AngularVelocity = 0f;
        }
        base.Update(time);
    }

    public void SetTouching(bool value)
    {
        if (touching != value)
        {
            touching = value;
        }
    }

    public bool TouchBegan(Touch touch)
    {
        if (Maths.Between(Body.Position.DistanceTo(builder.TouchRootVec(touch)) / clip.Scale, MIN_TOUCH_RADIUS, MAX_TOUCH_RADIUS))
        {
            this.touch = touch;
            startTouchAngle = VectorUtil.Atan2(Body.Position, builder.TouchRootVec(this.touch)) - Body.Rotation;
            SetTouching(value: true);
            StopActions();
            return true;
        }
        return false;
    }

    public bool TouchMove(Touch touch)
    {
        float num = targetAngle;
        RefreshAngle();
        lastDirection = targetAngle - num;
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    public void RefreshAngle()
    {
        targetAngle = VectorUtil.Atan2(Body.Position, builder.TouchRootVec(touch)) - startTouchAngle;
        targetAngle = targetAngle.SimplifyAngle(Body.Rotation - (float)Math.PI);
    }

    public void TouchEnd(Touch touch)
    {
        touchPointSpeed = lastPointSpeed;
        SetTouching(value: false);
        float num = Maths.Round(targetAngle, ANGLE_REMAINDER);
        targetAngle = Math.Abs(num - targetAngle) < ANGLE_REMAINDER / 5f
            ? num
            : lastDirection < 0f ? Maths.Floor(targetAngle, ANGLE_REMAINDER) : Maths.Ceil(targetAngle, ANGLE_REMAINDER);
        RunActions();
        this.touch = null;
    }
}
