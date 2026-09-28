using System;
using System.Collections.Generic;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Input;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class PlasticinePartBodyClip : ContreJourBodyClip, IClickable, IRestartable, IGrassControllerContainer
{
    private const float FIX_LIMIT_OFFSET = 2f / 3f;

    private const float GROUND_FALL_OFFSET = 2f / 3f;

    private const float GROUND_FALL_TIMEOUT = 0.205f;

    private const float SPEED_MULT = 0.5f;

    private const float MAX_GROUND_VELOCITY = 4.5f;

    private const float MOVE_SPEED_MULT = 0.4f;

    private const float MAX_X_SPEED = 4f;

    private const float JUMP_SPEED_MULT = 1f / 3f;

    private const float JUMP_FORCE_MULT = 50f;

    private const float MAX_JUMP_SPEED = 5f;

    private const float MOVE_FORCE = 1.2f;

    private const float DUST_OFFSET = 0.2f;

    public const float GRASS_TRAMPLE_DISTANCE = 1.3333334f;

    private const float GRASS_ANGLE_WHITE = (float)Math.PI / 3f;

    private const float GRASS_ANGLE = (float)Math.PI / 5f;

    private const float GRASS_RANDOM = 0.55f;

    private const float MIN_DUST_SPEED = 1f;

    private const float DUST_RANDOM = 0.33f;

    protected int globalIndex;

    protected int index;

    protected int verticesOffset;

    protected bool dirty;

    private PlasticineWideBorder border;

    private PlasticineSprite fillSprite;

    private int fillIndex = -1;

    protected PlasticinePartHighlite highlite;

    protected Vector2 initialPosition;

    protected float initialAngle;

    protected Vector2 targetPosition;

    protected float targetAngle;

    protected ContreJourGame game;

    protected List<DustData> dust = new List<DustData>(64);

    protected PlasticineItem item;

    protected bool updateParent;

    protected PlasticineBodyClip parent;

    protected float width;

    protected float groundFallTime;

    protected float groundFallMaxTime;

    protected bool isFloor;

    protected bool isTop;

    protected int dynamic;

    protected bool moving;

    protected bool dragging;

    protected float lastTime;

    protected Vector2 moveForce;

    protected Vector2 lastFrameSpeed;

    protected Vector2 normal;

    protected Vector2 parallel;

    protected IGrassController grassController;

    protected PlasticinePartBodyClip previous;

    protected PlasticinePartBodyClip next;

    protected bool isRotationDirty;

    protected Vector2 fixPosition;

    protected bool fixHighlite;

    protected Sprite circle;

    protected Vector2 circlePosition;

    protected float circleSize;

    protected float circleScale;

    private static int i;

    public PlasticineItem Item
    {
        get
        {
            return item;
        }
        set
        {
            item = value;
        }
    }

    public bool UpdateParent
    {
        get
        {
            return updateParent;
        }
        set
        {
            updateParent = value;
        }
    }

    public PlasticineBodyClip Parent => parent;

    public PlasticinePartHighlite Highlite
    {
        get
        {
            return highlite;
        }
        set
        {
            highlite = value;
        }
    }

    public bool Dragging
    {
        get
        {
            return dragging;
        }
        set
        {
            if (value != dragging)
            {
                dragging = value;
                if (!dragging)
                {
                    groundFallTime = 0f;
                    FallGround();
                }
            }
        }
    }

    public Vector2 Normal => normal;

    public Vector2 Parallel => parallel;

    public bool IsRotationDirty
    {
        get
        {
            return isRotationDirty;
        }
        set
        {
            isRotationDirty = value;
        }
    }

    public float InitialAngle => initialAngle;

    public int Index => index;

    public IGrassController GrassController => grassController;

    public float Width => width;

    public bool DisableHeroFocus => true;

    public PlasticinePartBodyClip(LevelBuilderBase _builder, object _body, PlasticineBodyClip _parent, float _width, bool hasGrass)
        : base(_builder, _body, null, null)
    {
        //IL_0046: Unknown result type (might be due to invalid IL or missing references)
        //IL_0183: Unknown result type (might be due to invalid IL or missing references)
        //IL_0189: Expected O, but got Unknown
        //IL_00d5: Unknown result type (might be due to invalid IL or missing references)
        //IL_00df: Expected O, but got Unknown
        width = _width;
        builder = _builder;
        parent = _parent;
        game = (ContreJourGame)_builder.Game;
        float value = ((Body)_body).Rotation.SimplifyAngle(-(float)Math.PI / 2f);
        initialPosition = Body.Position;
        targetPosition = initialPosition;
        targetAngle = Body.Rotation;
        initialAngle = Body.Rotation;
        float num = (game.WhiteSide ? ((float)Math.PI / 3f) : ((float)Math.PI / 5f));
        if (Maths.Between(value, 0f - num, num))
        {
            isFloor = true;
            if (hasGrass)
            {
                CreateGrass((ContreJourLevelBuilder)_builder, _parent, (Body)_body);
            }
        }
        globalIndex = i++;
        dirty = true;
        groundFallMaxTime = Maths.Random(0.205f, 0.41f);
        groundFallTime = 0f;
        isTop = Maths.Between(value, (float)Math.PI / 2f, 4.712389f);
        normal = GetSurfaceCenter() - Body.WorldCenter;
        parallel = Body.GetWorldPoint(new Vector2(1f, 0f)) - Body.WorldCenter;
        parent = _parent;
        PlasticineConstants.ApplyStaticBodiesFilter((Body)_body);
        dynamic = 0;
    }

    public void MoveToInitialPosition()
    {
        SetTargetPositionAngle(initialPosition, initialAngle);
    }

    public void Restart()
    {
        if (updateParent)
        {
            parent.Restart();
        }
    }

    public void SetFillSprite(PlasticineSprite fillSprite, int fillIndex)
    {
        this.fillSprite = fillSprite;
        this.fillIndex = fillIndex;
    }

    public void SetWideBorder(PlasticineWideBorder _border, int _index)
    {
        index = _index;
        verticesOffset = index * 2 * 2 + 2;
        border = _border;
    }

    public bool UseForZoom()
    {
        return true;
    }

    public bool AcceptFreeTouches()
    {
        return true;
    }

    public void SetDirty()
    {
        parent.Changed = true;
        SetDirtyNoSibling();
        if (next != null)
        {
            next.SetDirtyNoSibling();
        }
        if (previous != null)
        {
            previous.SetDirtyNoSibling();
        }
    }

    public void SetDirtyNoSibling()
    {
        dirty = true;
        if (highlite != null)
        {
            highlite.SetDirty();
        }
    }

    public int Priority(Vector2 touchPoint)
    {
        return 0;
    }

    public bool TouchBegan(Touch touch)
    {
        return parent.StartDragItemTouch(item, touch);
    }

    public bool TouchMove(Touch touch)
    {
        return parent.TouchMove(touch);
    }

    public void TouchOut(Touch touch)
    {
    }

    public void TouchEnd(Touch touch)
    {
        parent.StopDragTouch(item, touch);
    }

    public void Free(Touch touch)
    {
        parent.StopDragTouch(item, touch);
        game.FreeTouch(touch);
    }

    public void SetTargetPositionAngle(Vector2 position, float angle)
    {
        if (!(Body.Position == position) || !Maths.FuzzyEquals(angle, Body.Rotation))
        {
            moving = true;
            lastFrameSpeed = position - Body.Position;
            lastFrameSpeed *= 1f / lastTime;
            targetAngle = angle.SimplifyAngle(Body.Rotation - (float)Math.PI);
            targetPosition = position;
            if (lastFrameSpeed.Length() > 2f)
            {
                FallGround();
            }
        }
    }

    public void FixTouchingSpeed(Vector2 currentSpeed)
    {
        for (ContactEdge contactList = Body.ContactList; contactList != null; contactList = contactList.Next)
        {
            if (contactList.Contact.IsTouching)
            {
                Vector2 vector = currentSpeed;
                vector *= 50f * contactList.Other.Mass;
                contactList.Other.ApplyForce(vector, contactList.Other.WorldCenter);
            }
        }
    }

    public void FallGround()
    {
        groundFallTime -= lastTime;
        if (isTop && groundFallTime <= 0f && lastFrameSpeed.Y > -5f && Maths.Random() < 0.7f)
        {
            groundFallTime = groundFallMaxTime;
            Vector2 position = VectorUtil.Random(item.GetLeftOffset(2f / 3f), item.GetRightOffset(0f));
            GravityParticle gravityParticle = (GravityParticle)game.GroundFall.AddOrGetInvisible();
            if (lastFrameSpeed.Y < 0f)
            {
                gravityParticle.Speed = new Vector2(gravityParticle.Speed.X, gravityParticle.Speed.Y * 1.3f);
                position.Y += Math.Max(lastFrameSpeed.Y, -35f);
            }
            gravityParticle.Position = position;
        }
    }

    public override void Update(float time)
    {
        lastTime = time;
        if (previous == null)
        {
            previous = item.PreviousItem.BodyClip;
        }
        if (next == null)
        {
            next = item.NextItem.BodyClip;
            SetDirty();
        }
        MoveToTargetPosition(time);
        if (grassController != null)
        {
            grassController.Update(time);
        }
        if (dust.Count > 0)
        {
            List<object> list = new List<object>();
            foreach (DustData item in dust)
            {
                if (dragging)
                {
                    item.Dragging = true;
                }
                item.Update(time);
                if (item.ShouldRemove)
                {
                    list.Add(item);
                }
            }
            dust.RemoveList(list);
        }
        if (dragging && circle != null)
        {
            UpdateCircle();
        }
    }

    public void UpdateWideBorderAndFill()
    {
        if (border != null && dirty)
        {
            VertexPositionColor[] inBorder = border.InBorder;
            VertexPositionColor[] outBorder = border.OutBorder;
            if (index == 0)
            {
                inBorder[0].Position = item.GetSurfaceCenter().ToVector3();
                inBorder[1].Position = item.GetCenterOffset(-5f / 12f).ToVector3();
                outBorder[0].Position = item.GetSurfaceCenter().ToVector3();
                outBorder[1].Position = item.GetCenterOffset(0.65f).ToVector3();
            }
            SetBezierPointsOffsetIndexOffset(inBorder, 7f / 12f, 0);
            SetBezierPointsOffsetIndexOffset(inBorder, -5f / 12f, 1);
            SetBezierPointsOffsetIndexOffset(outBorder, 7f / 12f, 0);
            SetBezierPointsOffsetIndexOffset(outBorder, 0.65f, 1);
            if (fillSprite != null)
            {
                fillSprite.Vertices[fillIndex].Position = item.GetCenterOffset(-5f / 48f).ToVector3();
            }
            dirty = false;
        }
    }

    public void SetBezierPointsOffsetIndexOffset(VertexPositionColor[] vector, float offset, int indexOffset)
    {
        Vector2 centerOffset = GetCenterOffset(offset);
        Vector2 centerOffset2 = item.NextItem.BodyClip.GetCenterOffset(offset);
        Vector2 vec = VectorUtil.Center(GetRightOffset(offset), VectorUtil.Center(centerOffset, centerOffset2));
        vector[verticesOffset + indexOffset].Position = builder.ToPoint(vec).ToVector3();
        vector[verticesOffset + indexOffset + 2].Position = builder.ToPoint(centerOffset2).ToVector3();
    }

    public void MoveToTargetPosition(float time)
    {
        if (Maths.FuzzyNotEquals(targetAngle, Body.Rotation))
        {
            Body.SetTransform(Body.Position, targetAngle);
        }
        if (!targetPosition.FuzzyEquals(Body.Position, 0.01f))
        {
            Vector2 vector = targetPosition - Body.Position;
            Vector2 linearVelocity = vector;
            vector *= 0.4f;
            linearVelocity *= 0.5f / time;
            float num = linearVelocity.Length();
            if (num > 4.5f)
            {
                linearVelocity *= 4.5f / num;
            }
            Vector2 vector2 = Body.Position + vector;
            Body.LinearVelocity = linearVelocity;
            Body.SetTransform(vector2, Body.Rotation);
            SetRotationDirty();
            if (isTop)
            {
                FallGround();
            }
        }
        else if (Body.LinearVelocity != Vector2.Zero)
        {
            Body.LinearVelocity = Vector2.Zero;
            SetRotationDirty();
        }
        else if (isRotationDirty)
        {
            SetDirty();
            isRotationDirty = false;
            fixHighlite = true;
        }
        if (fixHighlite)
        {
            SetDirty();
            fixHighlite = false;
        }
    }

    public void UpdateCircle()
    {
        float num = VectorUtil.Projection(Body.Position - initialPosition, normal) / builder.EngineConfig.SizeMultiplier;
        float num2 = ((num > 0f) ? (num / circleSize / 2f) : 0f);
        circle.Scale = circleScale + num2;
    }

    public void SetRotationDirty()
    {
        SetDirty();
        isRotationDirty = true;
        previous.IsRotationDirty = true;
        next.IsRotationDirty = true;
    }

    public void OnTouchWith(float offset, BodyClip objectP)
    {
        if (grassController != null)
        {
            grassController.OnTouchWith(offset, objectP);
        }
    }

    public void ScareFlyes(int offset)
    {
        if (grassController != null)
        {
            grassController.ScareFlyes(offset);
        }
    }

    public void CreateGrass(ContreJourLevelBuilder _builder, PlasticineBodyClip _parent, Body _body)
    {
        if (!_builder.ContreJour.BlackSide)
        {
            grassController = (GrassController)ReflectUtil.CreateInstance(_builder.ContreJour.ChooseSide(null, typeof(WhiteGrassController), typeof(WhiteGrassController), typeof(GrassController), typeof(WhiteGrassController)), this);
        }
    }

    public Vector2 GetLocalSurfaceCenter()
    {
        return new Vector2(0f, 7f / 12f);
    }

    public Vector2 GetSurfaceCenter()
    {
        return Body.GetWorldPoint(GetLocalSurfaceCenter());
    }

    public Vector2 GetLeftOffset(float offset)
    {
        return Body.GetWorldPoint(new Vector2((0f - width) / 2f, offset));
    }

    public Vector2 GetRightOffset(float offset)
    {
        return Body.GetWorldPoint(new Vector2(width / 2f, offset));
    }

    public Vector2 GetCenterOffset(float offset)
    {
        return Body.GetWorldPoint(new Vector2(0f, offset));
    }

    public override void OnCollisionPoint(Body body2, Contact point)
    {
        BodyClip bodyClip = (BodyClip)body2.UserData;
        if (bodyClip != null)
        {
            Vector2 vector = default(Vector2);
            FixedArray2<Vector2> val = default(FixedArray2<Vector2>);
            point.GetWorldManifold(out vector, out val);
            Vector2 localPoint = Body.GetLocalPoint(val[0]);
            float x = localPoint.X;
            item.UpdateTouchesBodyClipDistance(x, bodyClip, 1.3333334f);
            localPoint.Y += 0.2f;
            if (isFloor && !dragging && bodyClip.Config.GetBool("hasDust"))
            {
                AddDustPointPosition(body2, point, localPoint);
            }
        }
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
    }

    public void AddDustPointPosition(Body body2, Contact point, Vector2 position)
    {
        float num = body2.LinearVelocity.Length();
        if (num >= 1f && Maths.Random() < 0.33f)
        {
            position = Body.GetWorldPoint(position);
            DustData dustData = new DustData(game, body2.LinearVelocity, builder.ToIPadPoint(position), num, (!game.WhiteSide) ? 1 : 2);
            dust.Add(dustData);
        }
    }
}
