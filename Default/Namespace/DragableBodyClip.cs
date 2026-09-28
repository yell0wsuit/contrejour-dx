using System;
using System.Collections.Generic;

using ContreJour.Clips.common;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class DragableBodyClip : ContreJourBodyClip, IClickable, IRestartable, ISnotHolder
{
    public const float RESTART_SPEED = 26.666666f;

    public const float MAX_SPEED = 60f;

    public const float FIX_HERO_SPEED_X = 1f / 6f;

    public const float FIX_HERO_SPEED = -1.3333334f;

    public const float ALPHA_STEP = 5f;

    public const float MAX_ALPHA = 255f;

    public const float MIN_ALPHA = 150f;

    public const float SPEED_PROPORTION = 0.5f;

    public const float MOVE_PROPORTION = 0.5f;

    public const float FUZZY_PRECISSION = 1f / 30f;

    public const float TOUCH_DISTANCE_IPHONE = 2.3333333f;

    public const float TOUCH_DISTANCE = 1.6666666f;

    public const float OFFSET = 3.4f;

    protected Vector2 axis;

    protected float currentAlpha;

    protected EventSender dragStartEvent;

    protected bool draging;

    protected Vector2 initialDragOffset;

    protected Vector2 initialMousePosition;

    protected Vector2 initialPosition;

    protected bool limitSpeed;

    protected float lowerLimit;

    protected McDragLimit middle;

    protected Vector2 targetPosition;

    protected Touch touch;

    protected float upperLimit;

    private readonly CircleShape _dragShape;

    private RectangleFloat _dragBounds;

    public EventSender DragStartEvent => dragStartEvent;

    public SnotBodyClip Snot { get; set; }

    public bool DisableHeroFocus => false;

    public virtual Vector2 SnotPosition
    {
        get
        {
            Vector2 vector = VectorExtensions.Rotate(builder.ToVec(new Vector2(0f, -60f)), 0f - rotationOffsetRadians);
            return Body.GetWorldPoint(vector);
        }
    }

    public DragableBodyClip(ContreJourLevelBuilder builder, object body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        //IL_00f7: Unknown result type (might be due to invalid IL or missing references)
        //IL_0101: Expected O, but got Unknown
        clip = LevelBuilderBase.ReplaceClipWith(clip, ReplaceClipName(builder));
        this.clip = clip;
        clip.Parent.ChangeChildLayer(clip, 2);
        dragStartEvent = new EventSender();
        float num = config.GetFloat("scaleX");
        initialPosition = Body.Position;
        targetPosition = initialPosition;
        upperLimit = 3.4f * num;
        lowerLimit = -3.4f * num;
        axis = VectorUtil.ToVector(1f, MathHelper.ToRadians(0f - this.config.GetFloat("rotation")));
        CreateBoundsClip(num);
        SetAlpha(150f);
        Body.BodyType = (BodyType)1;
        _dragShape = (CircleShape)Body.FixtureList.First(f => !((CircleShape)f.Shape).Position.FuzzyEquals(Vector2.Zero, 0.1f)).Shape;
    }

    public bool UseForZoom()
    {
        return false;
    }

    public bool AcceptFreeTouches()
    {
        return true;
    }

    protected override void FirstUpdate()
    {
        _dragBounds = Game.LevelScreenPhysicsBounds;
        _dragBounds.Offset(-_dragShape.Position);
        _dragBounds.Extend(-2f / 3f);
    }

    public int Priority(Vector2 touchPosition)
    {
        return 1;
    }

    public bool TouchBegan(Touch touch)
    {
        if (this.touch == null && ProcessTouch(touch))
        {
            limitSpeed = false;
            this.touch = touch;
            dragStartEvent.SendEvent();
            ContreJourGame contreJourGame = (ContreJourGame)builder.Game;
            contreJourGame.IncreaseZoomOut();
            Schedule(ContreJourGame.FocusOnHero, 0.05f);
            draging = true;
            initialMousePosition = builder.TouchRootVec(this.touch);
            initialDragOffset = Body.Position - initialPosition;
            return true;
        }
        return false;
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
        draging = false;
        builder.Game.DecreaseZoomOut();
        this.touch = null;
    }

    public void Restart()
    {
        targetPosition = initialPosition;
        limitSpeed = true;
    }

    protected virtual string ReplaceClipName(ContreJourLevelBuilder builder)
    {
        return builder.ContreJour.ChooseSide(null, "McDragViewWhite", "McDragView_5", "McDragView_5");
    }

    protected virtual void CreateBoundsClip(float scale)
    {
        middle = new McDragLimit();
        Vector2 vector = VectorUtil.ToVector(upperLimit / (1f / 30f), MathHelper.ToRadians(clip.RotationDegrees));
        Vector2 position = clip.Position;
        builder.AddChildBefore(middle, clip);
        float rotationDegrees = clip.RotationDegrees;
        middle.Position = position;
        middle.RotationDegrees = rotationDegrees;
        middle.ScaleX = (vector.Length() + 40f) * 2f / middle.Size.X;
    }

    protected virtual Vector2 TouchOffset()
    {
        return Vector2.Zero;
    }

    public void SetAlpha(float value)
    {
        if (Maths.FuzzyNotEquals(currentAlpha, value))
        {
            currentAlpha = value;
            RefreshObjectsAlpha();
        }
    }

    protected virtual void RefreshObjectsAlpha()
    {
        middle.OpacityByte = (int)currentAlpha;
    }

    public bool ProcessTouch(Touch touch)
    {
        return builder.TouchRootVec(touch).DistanceTo(Body.Position + TouchOffset()) < 2.3333333f;
    }

    public void UpdateTouchPosition()
    {
        targetPosition = GetDragPosition(builder.TouchRootVec(touch) - initialMousePosition + initialDragOffset);
        targetPosition = _dragBounds.ClampToBounds(targetPosition);
    }

    private void MoveToTarget(float time)
    {
        if (Maths.FuzzyNotEquals(time, 0f) && !targetPosition.FuzzyEquals(Body.Position, 1f / 30f))
        {
            Vector2 vec = targetPosition - Body.Position;
            _ = limitSpeed ? VectorUtil.ClampLength(ref vec, 26.666666f * time) : VectorUtil.ClampLength(ref vec, 60f * time);
            Vector2 linearVelocity = vec;
            linearVelocity *= 0.5f / time;
            vec *= 0.5f;
            Body.LinearVelocity = linearVelocity;
            Body.SetTransform(vec + Body.Position, Body.Rotation);
        }
    }

    protected virtual Vector2 GetDragPosition(Vector2 offset)
    {
        float num = Maths.Clamp(VectorUtil.Projection(offset, axis), lowerLimit, upperLimit);
        return (axis * num) + initialPosition;
    }

    public override void Update(float time)
    {
        //IL_003e: Unknown result type (might be due to invalid IL or missing references)
        //IL_0044: Invalid comparison between Unknown and I4
        Body.LinearVelocity = Vector2.Zero;
        if (touch != null)
        {
            UpdateTouchPosition();
        }
        MoveToTarget(time);
        if (Maths.FuzzyNotEquals(time, 0f) && (int)Body.BodyType == 2)
        {
            Body.BodyType = 0;
            Body.SetTransform(initialPosition, Body.Rotation);
        }
        SetAlpha(Maths.StepTo(target: draging ? 255f : 150f, value: currentAlpha, maxStep: 5f));
        base.Update(time);
    }
}
