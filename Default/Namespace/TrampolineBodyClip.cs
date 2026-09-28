using System;
using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class TrampolineBodyClip : SnotBodyClipBase
{
    public readonly EventSender DragEvent = new();

    public readonly EventSender HeroTouchEvent = new();

    protected Vector2 center;

    protected float centerDistanceDiff;

    protected FixedMouseJoint dragJoint;

    protected Vector2 dragOffset;

    protected bool dragging;

    protected float impulseMultiplier;

    protected Vector2 impulseVec;

    protected Vector2 initialPosition;

    protected List<Body> launchBodies = [];

    protected float maxDistance;

    protected Vector2 normal;

    protected TrampolinePartBodyClip part;

    private readonly Trajectory trajectory;

    protected float startDistance;

    protected float startTrampolineWidth;

    protected float timeFromLaunch;

    protected Touch touch;

    public bool Dragging
    {
        get => dragging;
        set => dragging = value;
    }

    public override Body Body
    {
        protected set
        {
            if (value != null && value.UserData != null)
            {
                part = (TrampolinePartBodyClip)value.UserData;
                value.UserData = null;
            }
            base.Body = value;
        }
    }

    public TrampolineBodyClip(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
        : base(builder, body, clip, config)
    {
        for (int i = 2; i < Physics.BodiesSize() - 2; i++)
        {
            if (Physics.BodyAt(i).UserData is TrampolinePartBodyClip trampolinePartBodyClip)
            {
                trampolinePartBodyClip.Parent = this;
            }
        }
        part.Parent = this;
        center = Physics.GetWorldStartPoint() + Physics.EndBody.Position;
        startTrampolineWidth = (Physics.GetWorldStartPoint() - Physics.EndBody.Position).Length();
        center *= 0.5f;
        normal = Physics.EndBody.Position - Physics.GetWorldStartPoint();
        normal = normal.Rotate90();
        normal *= 1f / normal.Length();
        impulseMultiplier = startTrampolineWidth / 6.533333f;
        startDistance = impulseMultiplier * 1.621671f;
        maxDistance = impulseMultiplier * 5f;
        centerDistanceDiff = maxDistance - startDistance;
        trajectory = new Trajectory(game)
        {
            Impulse = impulseMultiplier
        };
        this.builder.Add(trajectory, 11);
        trajectory.Position = this.builder.ToIPadPoint(center);
        trajectory.Angle = config.GetFloat("rotation").ToRadians() + ((float)Math.PI / 2f);
        timeFromLaunch = 0.3f;
        SetJointsDamping(1f);
    }

    protected override MonsterEye CreateEye()
    {
        return null;
    }

    public Vector2 CenterBodyPosition()
    {
        return CenterBody().WorldCenter;
    }

    public Body CenterBody()
    {
        return Physics.BodyAt(Physics.BodiesSize() / 2);
    }

    public void StartDrag(Touch touch)
    {
        this.touch = touch;
        DragEvent.SendEvent();
        dragging = true;
        Body val = CenterBody();
        dragJoint = JointFactory.CreateFixedMouseJoint(builder.World, val, val.WorldCenter);
        dragJoint.MaxForce = 500f;
        dragJoint.Frequency = 100f;
        game.IncreaseZoomOut();
    }

    public void EndDrag()
    {
        if (touch != null)
        {
            Launch();
            StopDrag();
        }
    }

    public void StopDrag()
    {
        if (dragging)
        {
            game.DecreaseZoomOut();
            dragging = false;
            builder.World.RemoveJoint((Joint)(object)dragJoint);
            dragJoint = null;
        }
        trajectory.Enabled = false;
    }

    public override void AddClipsToStage()
    {
        container.AddChild(clipContent);
        container.AddChild(baseEndClip);
        container.AddChild(baseClip);
        builder.Add(container, Layer());
    }

    public override void Update(float time)
    {
        base.Update(time);
        part?.Update(time);
        timeFromLaunch += time;
        if (dragging)
        {
            Vector2 vector = builder.TouchRootVec(touch);
            vector -= center;
            if (VectorUtil.Projection(vector, normal) > -1.621671f)
            {
                game.FreeTouch(touch);
                StopDrag();
            }
            else
            {
                UpdateDragPositionTime(vector, time);
            }
        }
    }

    public void UpdateDragPositionTime(Vector2 dragTarget, float time)
    {
        Vector2 worldCenter = CenterBody().WorldCenter;
        float num = dragTarget.Length();
        if (num > maxDistance)
        {
            dragTarget *= maxDistance / dragTarget.Length();
            num = maxDistance;
        }
        trajectory.Angle = VectorUtil.Atan2(dragTarget) + (float)Math.PI;
        trajectory.Impulse = Math.Max(num - 1.621671f, 0f) * impulseMultiplier;
        trajectory.Enabled = trajectory.Impulse > impulseMultiplier;
        dragTarget += center;
        dragTarget = VectorUtil.StepTo(worldCenter, dragTarget, 1.6666666f);
        dragJoint.WorldAnchorB = dragTarget;
        for (int i = 0; i < Physics.BodiesSize(); i++)
        {
            for (ContactEdge val = Physics.BodyAt(i).ContactList; val != null; val = val.Next)
            {
                if (!val.Other.FixtureList[0].IsSensor)
                {
                    Vector2 vector = CenterBodyPosition() - val.Other.WorldCenter;
                    vector *= 1f / vector.Length();
                    vector *= 3f * val.Other.Mass * time * 30f;
                    val.Other.ApplyForce(vector, val.Other.WorldCenter);
                }
            }
        }
    }

    public void Launch()
    {
        SoundManager.PlaySound("landing3", 0.4f);
        Vector2 direction = center - CenterBodyPosition();
        float num = direction.Length();
        if (num <= 1.621671f)
        {
            return;
        }
        List<Body> list = [];
        launchBodies.Clear();
        for (int i = 0; i < Physics.BodiesSize(); i++)
        {
            Body val = Physics.BodyAt(i);
            ContactEdge val2 = val.ContactList;
            float num2 = (val.WorldCenter - center).Length();
            list.Add(val);
            while (val2 != null)
            {
                if (val2.Contact.IsTouching && (val2.Other.WorldCenter - center).Length() < num2)
                {
                    launchBodies.Add(val2.Other);
                }
                val2 = val2.Next;
            }
        }
        direction *= 1f / num;
        float num3 = (num - startDistance) / centerDistanceDiff * impulseMultiplier;
        LaunchBodiesImpulseDirection(list, 6f * num3, direction);
        LaunchBodiesImpulseDirection(launchBodies, 22f * num3, direction);
        impulseVec = direction;
        impulseVec *= 22f * num3;
        timeFromLaunch = 0f;
    }

    public void OnCollisionStart(Body launchBody)
    {
        if (HeroTouchEvent.Enabled && launchBody.UserData is HeroBodyClip)
        {
            HeroTouchEvent.SendEvent();
        }
        if (timeFromLaunch < 0.3f && launchBodies.NotExists(launchBody))
        {
            launchBodies.Add(launchBody);
            Vector2 vector = impulseVec;
            vector *= launchBody.Mass;
            launchBody.LinearVelocity = new Vector2(0f, 0f);
            launchBody.ApplyLinearImpulse(vector, launchBody.WorldCenter);
            UpdateAchievement(launchBody);
        }
    }

    public static void UpdateAchievement(Body launchBody)
    {
        _ = launchBody.UserData;
    }

    public static void LaunchBodiesImpulseDirection(List<Body> bodies, float impulse, Vector2 direction)
    {
        foreach (Body body in bodies)
        {
            Vector2 vector = direction;
            vector *= impulse * body.Mass;
            body.LinearVelocity = new Vector2(0f, 0f);
            body.ApplyLinearImpulse(vector, body.WorldCenter);
            UpdateAchievement(body);
        }
    }

    public void SetJointsDamping(float value)
    {
        //IL_0010: Unknown result type (might be due to invalid IL or missing references)
        //IL_0016: Expected O, but got Unknown
        for (int i = 1; i < Physics.JoitsSize; i++)
        {
            ((DistanceJoint)Physics.JointAt(i)).DampingRatio = value;
        }
    }

    public override void InitSizes()
    {
        base.InitSizes();
        startWidthPixels = 14f;
        startWidth = startWidthPixels * builder.EngineConfig.SizeMultiplier;
        endWidthPixels = startWidth;
        endWidth = startWidth;
    }

    public override SnotSprite CreateClip()
    {
        return game.WhiteSide
            ? new WhiteTrampolineSprite(game, this, startWidth, centerWidth, endWidth)
            : game.BlackSide
            ? new BlackTrampolineSprite(game, this, startWidth, centerWidth, endWidth)
            : new SnotSprite(this, startWidth, centerWidth, endWidth);
    }

    public override string BaseEndClipName()
    {
        return game.ChooseSide("McTrampolineEndBlack", "McTrampolineEndWhite", "McTrampolineEnd");
    }

    public override string BaseClipName()
    {
        return BaseEndClipName();
    }
}
