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

namespace Default.Namespace;

public class PlasticinePartBodyClip : ContreJourBodyClip, IClickable, IRestartable, IGrassControllerContainer
{
    public const float GrassTrampleDistance = 1.3333334f;
    private int verticesOffset;

    private bool dirty;

    private PlasticineWideBorder border;

    private PlasticineSprite fillSprite;

    private int fillIndex = -1;
    private Vector2 initialPosition;
    private Vector2 targetPosition;

    private float targetAngle;

    private readonly ContreJourGame game;

    private readonly List<DustData> dust = new(64);
    private float groundFallTime;

    private readonly float groundFallMaxTime;

    private readonly bool isFloor;

    private readonly bool isTop;
    private float lastTime;
    private Vector2 lastFrameSpeed;

    protected ref Vector2 LastFrameSpeed => ref lastFrameSpeed;

    private Vector2 normal;

    private Vector2 parallel;
    private PlasticinePartBodyClip previous;

    private PlasticinePartBodyClip next;
    private bool fixHighlite;


    public PlasticineItem Item { get; set; }

    public bool UpdateParent { get; set; }

    public PlasticineBodyClip Parent { get; }

    public PlasticinePartHighlite Highlite { get; set; }

    public bool Dragging
    {
        get; set
        {
            if (value != field)
            {
                field = value;
                if (!field)
                {
                    groundFallTime = 0f;
                    FallGround();
                }
            }
        }
    }

    public Vector2 Normal => normal;

    public Vector2 Parallel => parallel;

    public bool IsRotationDirty { get; set; }

    public float InitialAngle { get; }

    public int Index { get; private set; }

    public IGrassController GrassController { get; private set; }

    public float Width { get; }

    public bool DisableHeroFocus => true;

    public PlasticinePartBodyClip(LevelBuilderBase builder, object body, PlasticineBodyClip parent, float width, bool hasGrass)
        : base(builder, body, null, null)
    {
        //IL_0046: Unknown result type (might be due to invalid IL or missing references)
        //IL_0183: Unknown result type (might be due to invalid IL or missing references)
        //IL_0189: Expected O, but got Unknown
        //IL_00d5: Unknown result type (might be due to invalid IL or missing references)
        //IL_00df: Expected O, but got Unknown
        Width = width;
        Builder = builder;
        Parent = parent;
        game = (ContreJourGame)builder.Game;
        float value = ((Body)body).Rotation.SimplifyAngle(-(float)Math.PI / 2f);
        initialPosition = Body.Position;
        targetPosition = initialPosition;
        targetAngle = Body.Rotation;
        InitialAngle = Body.Rotation;
        float num = game.WhiteSide ? ((float)Math.PI / 3f) : ((float)Math.PI / 5f);
        if (Maths.Between(value, 0f - num, num))
        {
            isFloor = true;
            if (hasGrass)
            {
                CreateGrass((ContreJourLevelBuilder)builder);
            }
        }
        dirty = true;
        groundFallMaxTime = Maths.Random(0.205f, 0.41f);
        groundFallTime = 0f;
        isTop = Maths.Between(value, (float)Math.PI / 2f, 4.712389f);
        normal = GetSurfaceCenter() - Body.WorldCenter;
        parallel = Body.GetWorldPoint(new Vector2(1f, 0f)) - Body.WorldCenter;
        Parent = parent;
        PlasticineConstants.ApplyStaticBodiesFilter((Body)body);
    }

    public void MoveToInitialPosition()
    {
        SetTargetPositionAngle(initialPosition, InitialAngle);
    }

    public void Restart()
    {
        if (UpdateParent)
        {
            Parent.Restart();
        }
    }

    public void SetFillSprite(PlasticineSprite fillSprite, int fillIndex)
    {
        this.fillSprite = fillSprite;
        this.fillIndex = fillIndex;
    }

    public void SetWideBorder(PlasticineWideBorder border, int index)
    {
        Index = index;
        verticesOffset = (Index * 2 * 2) + 2;
        this.border = border;
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
        Parent.Changed = true;
        SetDirtyNoSibling();
        next?.SetDirtyNoSibling();
        previous?.SetDirtyNoSibling();
    }

    public void SetDirtyNoSibling()
    {
        dirty = true;
        Highlite?.SetDirty();
    }

    public int Priority(Vector2 touchPosition)
    {
        return 0;
    }

    public bool TouchBegan(Touch touch)
    {
        return Parent.StartDragItemTouch(Item, touch);
    }

    public bool TouchMove(Touch touch)
    {
        return Parent.TouchMove(touch);
    }

    public void TouchOut(Touch touch)
    {
    }

    public void TouchEnd(Touch touch)
    {
        Parent.StopDragTouch(touch);
    }

    public void Free(Touch touch)
    {
        Parent.StopDragTouch(touch);
        game.FreeTouch(touch);
    }

    public void SetTargetPositionAngle(Vector2 position, float angle)
    {
        if (!(Body.Position == position) || !Maths.FuzzyEquals(angle, Body.Rotation))
        {
            LastFrameSpeed = position - Body.Position;
            LastFrameSpeed *= 1f / lastTime;
            targetAngle = angle.SimplifyAngle(Body.Rotation - (float)Math.PI);
            targetPosition = position;
            if (LastFrameSpeed.Length() > 2f)
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
        if (isTop && groundFallTime <= 0f && LastFrameSpeed.Y > -5f && Maths.Random() < 0.7f)
        {
            groundFallTime = groundFallMaxTime;
            Vector2 position = VectorUtil.Random(Item.GetLeftOffset(2f / 3f), Item.GetRightOffset(0f));
            GravityParticle gravityParticle = (GravityParticle)game.GroundFall.AddOrGetInvisible();
            if (LastFrameSpeed.Y < 0f)
            {
                gravityParticle.Speed = new Vector2(gravityParticle.Speed.X, gravityParticle.Speed.Y * 1.3f);
                position.Y += Math.Max(LastFrameSpeed.Y, -35f);
            }
            gravityParticle.Position = position;
        }
    }

    public override void Update(float time)
    {
        lastTime = time;
        previous ??= Item.PreviousItem.BodyClip;
        if (next == null)
        {
            next = Item.NextItem.BodyClip;
            SetDirty();
        }
        MoveToTargetPosition(time);
        GrassController?.Update(time);
        if (dust.Count > 0)
        {
            List<object> list = [];
            foreach (DustData item in dust)
            {
                if (Dragging)
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
    }

    public void UpdateWideBorderAndFill()
    {
        if (border != null && dirty)
        {
            VertexPositionColor[] inBorder = border.InBorder;
            VertexPositionColor[] outBorder = border.OutBorder;
            if (Index == 0)
            {
                inBorder[0].Position = Item.GetSurfaceCenter().ToVector3();
                inBorder[1].Position = Item.GetCenterOffset(-5f / 12f).ToVector3();
                outBorder[0].Position = Item.GetSurfaceCenter().ToVector3();
                outBorder[1].Position = Item.GetCenterOffset(0.65f).ToVector3();
            }
            SetBezierPointsOffsetIndexOffset(inBorder, 7f / 12f, 0);
            SetBezierPointsOffsetIndexOffset(inBorder, -5f / 12f, 1);
            SetBezierPointsOffsetIndexOffset(outBorder, 7f / 12f, 0);
            SetBezierPointsOffsetIndexOffset(outBorder, 0.65f, 1);
            fillSprite?.Vertices[fillIndex].Position = Item.GetCenterOffset(-5f / 48f).ToVector3();
            dirty = false;
        }
    }

    public void SetBezierPointsOffsetIndexOffset(VertexPositionColor[] vector, float offset, int indexOffset)
    {
        Vector2 centerOffset = GetCenterOffset(offset);
        Vector2 centerOffset2 = Item.NextItem.BodyClip.GetCenterOffset(offset);
        Vector2 vec = VectorUtil.Center(GetRightOffset(offset), VectorUtil.Center(centerOffset, centerOffset2));
        vector[verticesOffset + indexOffset].Position = Builder.ToPoint(vec).ToVector3();
        vector[verticesOffset + indexOffset + 2].Position = Builder.ToPoint(centerOffset2).ToVector3();
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
        else if (IsRotationDirty)
        {
            SetDirty();
            IsRotationDirty = false;
            fixHighlite = true;
        }
        if (fixHighlite)
        {
            SetDirty();
            fixHighlite = false;
        }
    }

    public void SetRotationDirty()
    {
        SetDirty();
        IsRotationDirty = true;
        previous.IsRotationDirty = true;
        next.IsRotationDirty = true;
    }

    public void OnTouchWith(float offset, BodyClip objectP)
    {
        GrassController?.OnTouchWith(offset, objectP);
    }

    public void ScareFlyes(int offset)
    {
        GrassController?.ScareFlyes(offset);
    }

    public void CreateGrass(ContreJourLevelBuilder builder)
    {
        if (!builder.ContreJour.BlackSide)
        {
            GrassController = (GrassController)ReflectUtil.CreateInstance(builder.ContreJour.ChooseSide(null, typeof(WhiteGrassController), typeof(WhiteGrassController), typeof(GrassController), typeof(WhiteGrassController)), this);
        }
    }

    public static Vector2 GetLocalSurfaceCenter()
    {
        return new Vector2(0f, 7f / 12f);
    }

    public Vector2 GetSurfaceCenter()
    {
        return Body.GetWorldPoint(GetLocalSurfaceCenter());
    }

    public Vector2 GetLeftOffset(float offset)
    {
        return Body.GetWorldPoint(new Vector2((0f - Width) / 2f, offset));
    }

    public Vector2 GetRightOffset(float offset)
    {
        return Body.GetWorldPoint(new Vector2(Width / 2f, offset));
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
            point.GetWorldManifold(out _, out FixedArray2<Vector2> val);
            Vector2 localPoint = Body.GetLocalPoint(val[0]);
            float x = localPoint.X;
            Item.UpdateTouchesBodyClipDistance(x, bodyClip, 1.3333334f);
            localPoint.Y += 0.2f;
            if (isFloor && !Dragging && bodyClip.Config.GetBool("hasDust"))
            {
                AddDustPointPosition(body2, localPoint);
            }
        }
    }

    public override void OnCollisionStartPoint(Body body2, Contact point)
    {
    }

    public void AddDustPointPosition(Body body2, Vector2 position)
    {
        float num = body2.LinearVelocity.Length();
        if (num >= 1f && Maths.Random() < 0.33f)
        {
            position = Body.GetWorldPoint(position);
            DustData dustData = new(game, body2.LinearVelocity, Builder.ToIPadPoint(position), num, (!game.WhiteSide) ? 1 : 2);
            dust.Add(dustData);
        }
    }
}
