using System.Collections.Generic;

using ContreJour.Clips.common;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class SnotBodyClip : SnotBodyClipBase, IClickable, IVectorPositionProvider, IRestartable
{
    public class BodyAndPoint
    {
        public Body Body;

        public Vector2 Point;

        public BodyAndPoint(Body body, Vector2 point)
        {
            Body = body;
            Point = point;
        }
    }

    public class LinkableReqParams
    {
        public float Distance;

        public Vector2 Position;

        public LinkableReqParams(float distance, Vector2 position)
        {
            Distance = distance;
            Position = position;
        }
    }

    private readonly RevoluteJointDef eyeJointDef;

    protected BlackTail blackTail;

    protected bool blinking;

    protected bool dynamicDrag;

    protected bool hasRelease;

    protected Sprite highlite;

    protected CosChanger highliteChanger;

    protected bool jointRemoved;

    protected float length;

    protected EventSender linkEvent;

    protected ISnotLinked linked;

    protected EventSender releaseEvent;

    protected SnotEye snotEye;

    protected RevoluteJoint stickyJoint;

    protected Touch touch;

    protected float touchEndTime;

    private readonly bool movable;

    private bool enabled = true;

    private readonly DragableBodyClip joinedBodyClip;

    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled != value)
            {
                enabled = value;
                if (enabled)
                {
                    StopParts();
                }
                else
                {
                    ReleaseSnot();
                }
            }
        }
    }

    public EventSender LinkEvent => linkEvent;

    public EventSender ReleaseEvent => releaseEvent;

    public ISnotLinked Linked => linked;

    public bool Dragging => touch != null;

    public bool Joined => stickyJoint != null;

    protected virtual float MoveTeleportCoeff => 0.5f;

    public bool DisableHeroFocus => false;

    public override Vector2 PositionVec => Physics.EndBody.Position;

    public SnotBodyClip(LevelBuilderBase _builder, SnotData _body, Node _clip, Hashtable _config)
        : base(_builder, _body, _clip, _config)
    {
        length = Physics.InitialLength;
        linkEvent = new EventSender();
        releaseEvent = new EventSender();
        SetDamping(FreeDamping());
        movable = config.GetBool("movable");
        snotEye = movable
            ? new MovableSnotEye(targetPoint: new SnotPoint(builder, _body.EyeBody.Position, null, null)
            {
                Used = true
            }, _snot: this, _body: Physics.EyeBody)
            : new SnotEye(this, Physics.EyeBody);
        eyeJointDef = new RevoluteJointDef(Physics.EyeJoint);
        game.AddPositionProvider(new PositionProviderValue(this, 1f));
        stickyJoint = null;
        if (game.BlackSide || game.BonusChapter)
        {
            CreateTail();
        }
        touchEndTime = -1f;
        dynamicDrag = config.GetBool("dynamicDrag");
        if (game.LevelIndex == 169)
        {
            Schedule(Blink, Maths.Random(5f, 8f));
        }
        else
        {
            Schedule(Blink, Maths.Random(1f, 2f));
        }
        joinedBodyClip = Physics.JoinedBody.UserData as DragableBodyClip;
        if (joinedBodyClip != null)
        {
            joinedBodyClip.Snot = this;
            container.RemoveFromParent();
            joinedBodyClip.Clip.RemoveFromParent();
            int layer = joinedBodyClip.Clip.Layer;
            _ = builder.AddChild(joinedBodyClip.Clip, layer);
            _ = builder.AddChild(container, layer);
        }
    }

    public bool UseForZoom()
    {
        return false;
    }

    public bool AcceptFreeTouches()
    {
        return true;
    }

    public int Priority(Vector2 touchPosition)
    {
        return 1;
    }

    public virtual bool TouchBegan(Touch touch)
    {
        if (this.touch != null || stickyJoint != null || !Enabled)
        {
            return false;
        }
        Vector2 source = builder.TouchRootVec(touch);
        if (movable && source.DistanceTo(Body.Position) > source.DistanceTo(Physics.EyeBody.Position))
        {
            return false;
        }
        SetZ(Layer() + 1);
        this.touch = touch;
        Physics.EndBody.BodyType = 0;
        Physics.EndBody.LinearVelocity = default;
        SetDamping(3f);
        TryRemoveJoint();
        Physics.EndBody.SetTransform(GetDragTarget().Point, Physics.EndBody.Rotation);
        SoundManager.PlaySound("leapOn1", 0.5f);
        return true;
    }

    public void TouchEnd(Touch touch)
    {
        if (touch == this.touch)
        {
            EndDrag();
        }
    }

    public bool TouchMove(Touch touch)
    {
        return true;
    }

    public void TouchOut(Touch touch)
    {
    }

    public void Restart()
    {
        touchEndTime = -1f;
    }

    private static float ClosestReq(object item, object param)
    {
        BodyClip bodyClip = (BodyClip)item;
        return 0f - VectorExtensions.DistanceTo(target: (Vector2)param, source: bodyClip.Body.Position);
    }

    private static bool LinkableReq(BodyClip clip, object param)
    {
        //IL_000d: Unknown result type (might be due to invalid IL or missing references)
        LinkableReqParams linkableReqParams = (LinkableReqParams)param;
        return clip.Body.BodyType != 0 && linkableReqParams.Position.DistanceTo(clip.Body.Position) < linkableReqParams.Distance && clip is ISnotLinked && (clip as ISnotLinked).SnotEnabled;
    }

    public void SetLengthDiff(float value)
    {
        //IL_0037: Unknown result type (might be due to invalid IL or missing references)
        //IL_003d: Expected O, but got Unknown
        length = Physics.InitialLength + value;
        float num = length / Physics.JoitsSize;
        for (int i = 0; i < Physics.JoitsSize; i++)
        {
            ((DistanceJoint)Physics.JointAt(i)).Length = num;
        }
    }

    public static float ExtremeDamping()
    {
        return 6f;
    }

    public static float FreeDamping()
    {
        return 0.5f;
    }

    public virtual float JoinedDamping()
    {
        return 0f;
    }

    public virtual void CreateHighlite(ContreJourGame _game)
    {
        if (!_game.BlackSide && !game.WhiteSide && !game.BonusChapter)
        {
            highlite = new McSnotEndHighlite();
            highliteChanger = new CosChanger(0.05f, 0.1f);
        }
    }

    public void Blink()
    {
        Schedule(Blink, Maths.Random(10f, 25f));
        if (stickyJoint == null)
        {
            blinking = true;
            eye.Open = true;
            Schedule(EndBlink, Maths.Random(2f, 5f));
        }
    }

    public void EndBlink()
    {
        blinking = false;
        if (stickyJoint == null)
        {
            eye.Open = false;
        }
    }

    public virtual void CreateTail()
    {
        blackTail = new BlackTail(textureFile: game.BlackSide ? "snotTailTextureBlack" : "McTailTextureGreen", _body: Physics.EndBody, _builder: builder);
        builder.Add(blackTail, 3);
        blackTail.Width = 20f;
    }

    public override void AddClipsToStage()
    {
        CreateHighlite(game);
        if (highlite != null)
        {
            container.AddChild(highlite);
        }
        container.AddChild(clipContent);
        container.AddChild(baseEndClip);
        container.AddChild(baseClip);
        if (eye != null)
        {
            container.AddChild(eye);
        }
        builder.Add(container, Layer());
    }

    public override SnotSprite CreateClip()
    {
        ContreJourGame contreJourGame = (ContreJourGame)builder.Game;
        return (SnotSprite)ReflectUtil.CreateInstance(contreJourGame.ChooseSide(typeof(BlackSnotSprite), typeof(WhiteSnotSprite), typeof(SpringSnotSprite), typeof(SpringSnotSprite), typeof(GreenSnotSprite)), contreJourGame, this, startWidth, centerWidth, endWidth);
    }

    public override void Update(float time)
    {
        if (hasRelease)
        {
            ReleaseSnot();
            hasRelease = false;
        }
        if (Dragging)
        {
            for (int i = 0; i < Physics.BodiesSize(); i++)
            {
                Physics.BodyAt(i).Awake = true;
            }
        }
        if (stickyJoint == null && ((touchEndTime > 0f && game.TotalTime - touchEndTime < 0.3f) || Dragging))
        {
            if (Dragging)
            {
                UpdateDragBodyPosition(GetDragTarget(), time / 3f);
            }
            else
            {
                BodyAndPoint dragTarget = GetDragTarget(Physics.EndBody.Position);
                if (dragTarget.Body != null)
                {
                    UpdateDragBodyPosition(dragTarget, time / 3f);
                }
            }
            if (stickyJoint != null)
            {
                touchEndTime = 0f;
            }
        }
        ((SpringSnotSprite)clipContent).Active = Dragging || Joined || blinking;
        base.Update(time);
        if (highlite != null)
        {
            highlite.Position = baseEndClip.Position;
            highlite.OpacityByte = (int)(80f + (highliteChanger.Value * 80f));
            highliteChanger.Update(time);
        }
        if (movable)
        {
            baseClip.Position = builder.ToPoint(snotEye.Body.Position);
        }
        eye.Position = baseClip.Position;
        if (blackTail != null)
        {
            blackTail.Update(time);
            blackTail.Moving = stickyJoint != null;
        }
        clipContent.OpacityFloat = clipContent.OpacityFloat.StepTo(enabled ? 1f : 0.5f, 0.05f);
        baseEndClip.Color = Color.White * clipContent.OpacityFloat;
    }

    public override Vector2 EndPosition()
    {
        return stickyJoint == null ? base.EndPosition() : stickyJoint.WorldAnchorB;
    }

    protected virtual void UpdateDragBodyPosition(BodyAndPoint target, float time)
    {
        Physics.EndBody.Move(target.Point, Physics.EndBody.Rotation, MoveTeleportCoeff, time);
        if (target.Body != null)
        {
            JoinToPosition(target.Body, target.Point);
            EndDrag();
        }
    }

    public override string BaseEndClipName()
    {
        return ((ContreJourGame)builder.Game).ChooseSide("McSnotEndBlack", "McSnotEndWhite", "McSnotEnd", "McSnotEnd", "McSnotEnd_6");
    }

    public override string BaseClipName()
    {
        return ((ContreJourGame)builder.Game).ChooseSide("McSnotStartBlack", "McSnotStartWhite", "McSnotStart", "McSnotStart", "McSnotStart_6");
    }

    public virtual string[] OnSound()
    {
        return Sounds.LEAP_ON;
    }

    public virtual float DragDistanceMultiplier()
    {
        return 2.6f;
    }

    public static Vector2 OtherLocalAnchor(JointEdge edge)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000c: Expected O, but got Unknown
        RevoluteJoint val = (RevoluteJoint)edge.Joint;
        return val.BodyA == edge.Other ? val.BodyA.GetLocalPoint(val.LocalAnchorA) : val.BodyB.GetLocalPoint(val.LocalAnchorB);
    }

    public static Vector2 ThisLocalAnchor(JointEdge edge)
    {
        //IL_0006: Unknown result type (might be due to invalid IL or missing references)
        //IL_000c: Expected O, but got Unknown
        RevoluteJoint val = (RevoluteJoint)edge.Joint;
        return val.BodyA != edge.Other ? val.BodyA.GetLocalPoint(val.LocalAnchorA) : val.BodyB.GetLocalPoint(val.LocalAnchorB);
    }

    public float MaxLength()
    {
        return length * DragDistanceMultiplier();
    }

    public BodyAndPoint GetDragTarget()
    {
        return GetDragTarget(builder.TouchRootVec(touch).ClampDistance(Physics.EyeBody.Position, MaxLength()));
    }

    public BodyAndPoint GetDragTarget(Vector2 _position)
    {
        List<BodyClip> list = FarseerUtil.Query(param: new LinkableReqParams(JoinDistance(), _position), world: builder.World, center: _position, radius: JoinDistance(), clipPredicate: LinkableReq);
        if (list.Count == 0)
        {
            return new BodyAndPoint(null, _position);
        }
        BodyClip bodyClip = (BodyClip)Arrays.MaxItem(list, ClosestReq, _position);
        return new BodyAndPoint(bodyClip.Body, bodyClip.Body.Position);
    }

    public void SetZ(int value)
    {
        if (joinedBodyClip == null)
        {
            builder.ChangeChildLayer(container, value);
        }
    }

    public override int Layer()
    {
        return 4;
    }

    public void OnLinkedDestroy()
    {
        hasRelease = true;
    }

    public void ReleaseSnot()
    {
        if (stickyJoint != null)
        {
            SoundManager.PlayRandomSound(Sounds.LEAP_OUT, 0.5f);
            SetDamping(FreeDamping());
            if (linked != null)
            {
                linked.SnotJoinedCount--;
                linked.DestroyEvent.RemoveListener(OnLinkedDestroy);
                linked = null;
            }
            if (stickyJoint.BodyA != null && stickyJoint.BodyB != null)
            {
                builder.World.RemoveJoint((Joint)(object)stickyJoint);
            }
            stickyJoint = null;
            eye.Open = false;
            ApplyDisconnectForce();
            SetZ(Layer());
            releaseEvent.SendEvent();
        }
        if (touch != null)
        {
            EndDrag();
        }
    }

    public virtual void ApplyDisconnectForce()
    {
        Vector2 worldStartPoint = Physics.GetWorldStartPoint();
        worldStartPoint -= Physics.EndBody.Position;
        worldStartPoint *= 10f / worldStartPoint.Length();
        Physics.EndBody.ApplyForce(worldStartPoint, Physics.EndBody.WorldCenter);
    }

    public virtual float JoinDistance()
    {
        return 2f;
    }

    private void JoinToPosition(Body joinBody, Vector2 joinPoint)
    {
        BodyClip bodyClip = (BodyClip)joinBody.UserData;
        if (bodyClip is ISnotLinked)
        {
            linked = (ISnotLinked)bodyClip;
            linked.SnotJoinedCount++;
            linked.DestroyEvent.AddListener(OnLinkedDestroy);
        }
        SoundManager.PlayRandomSound(OnSound(), 0.5f);
        SetDamping(JoinedDamping());
        Physics.EndBody.SetTransform(joinPoint, Physics.EndBody.Rotation);
        Physics.EndBody.LinearVelocity = Vector2.Zero;
        stickyJoint = FarseerUtil.CreateRevoluteJoint(World, Physics.EndBody, joinBody, joinPoint);
        eye.Open = true;
        SetZ(Layer() + 1);
        ContreJourGame.FocusOnHero();
        linkEvent.SendEvent();
    }

    public static bool BodyConnectedToStaticProcessed(Body _body, ref List<Body> processed)
    {
        //IL_0001: Unknown result type (might be due to invalid IL or missing references)
        //IL_0007: Invalid comparison between Unknown and I4
        if ((int)_body.BodyType != 2)
        {
            return true;
        }
        for (JointEdge val = _body.JointList; val != null; val = val.Next)
        {
            if (processed.NotExists(val.Other))
            {
                processed.Add(val.Other);
                if (val.Other.UserData is BodyClip bodyClip && bodyClip is SnotBodyClip)
                {
                    if (((SnotBodyClip)bodyClip).ConnectedToStatic(ref processed))
                    {
                        return true;
                    }
                }
                else if (BodyConnectedToStaticProcessed(val.Other, ref processed))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsConnectedToStatic()
    {
        if (dynamicDrag)
        {
            return false;
        }
        List<Body> processed = [Physics.EyeBody];
        return BodyConnectedToStaticProcessed(Physics.EyeBody, ref processed);
    }

    public bool ConnectedToStatic(ref List<Body> processed)
    {
        bool flag = BodyConnectedToStaticProcessed(Physics.EyeBody, ref processed);
        return !flag && !Dragging ? BodyConnectedToStaticProcessed(Physics.EndBody, ref processed) : flag;
    }

    public void StopParts()
    {
        for (int i = 0; i < Physics.BodiesSize(); i++)
        {
            Body obj = Physics.BodyAt(i);
            obj.LinearVelocity *= 0.2f;
        }
    }

    public virtual void SetDamping(float value)
    {
        for (int i = 0; i < Physics.BodiesSize(); i++)
        {
            Physics.BodyAt(i).Awake = true;
            Physics.BodyAt(i).LinearDamping = value;
        }
    }

    public void TryRemoveJoint()
    {
        if (!IsConnectedToStatic() && !jointRemoved)
        {
            builder.World.RemoveJoint((Joint)(object)Physics.EyeJoint);
            Physics.EyeJoint = null;
            jointRemoved = true;
        }
    }

    public virtual void EndDrag()
    {
        if (Dragging)
        {
            touchEndTime = game.TotalTime;
            SetDamping(FreeDamping());
            Physics.EndBody.BodyType = (BodyType)2;
            SetZ(Layer());
            touch = null;
            if (jointRemoved)
            {
                jointRemoved = false;
                Physics.EyeBody.LinearVelocity = new Vector2(0f, 0f);
                Physics.EyeJoint = eyeJointDef.Create(builder.World);
            }
        }
    }
}
