using System;
using System.Collections.Generic;
using System.Numerics;

using ContreJourDX.Clips;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Events;
using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Sound;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class SnotBodyClip : SnotBodyClipBase, IClickable, IVectorPositionProvider, IRestartable
    {
        public class BodyAndPoint(Body body, Vector2 point)
        {
            public Body Body { get; set; } = body;

            public Vector2 Point { get; set; } = point;
        }

        public class LinkableReqParams(float distance, Vector2 position)
        {
            public float Distance { get; set; } = distance;

            public Vector2 Position { get; set; } = position;
        }

        private readonly RevoluteJointDef eyeJointDef;

        private BlackTail blackTail;

        private bool blinking;

        private readonly bool dynamicDrag;

        private bool hasRelease;

        private Sprite highlite;

        private CosChanger highliteChanger;

        private bool jointRemoved;

        private float length;
        public ISnotLinked Linked { get; private set; }
        private readonly SnotEye snotEye;

        protected RevoluteJoint StickyJoint { get; set; }

        private Touch touch;

        private float touchEndTime;

        private readonly bool movable;
        private readonly DragableBodyClip joinedBodyClip;

        public bool Enabled
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    if (field)
                    {
                        StopParts();
                    }
                    else
                    {
                        ReleaseSnot();
                    }
                }
            }
        } = true;

        public EventSender LinkEvent { get; }

        public EventSender ReleaseEvent { get; }

        public bool Dragging => touch != null;

        public bool Joined => StickyJoint != null;

        protected virtual float MoveTeleportCoeff => 0.5f;

        public bool DisableHeroFocus => false;

        public override Vector2 PositionVec => Physics.EndBody.Position;

        public override Vector2 StartPosition => Physics.EyeBody.Position;

        public SnotBodyClip(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            length = Physics.InitialLength;
            LinkEvent = new EventSender();
            ReleaseEvent = new EventSender();
            SetDamping(FreeDamping());
            movable = Config.GetBool("movable");
            snotEye = movable
                ? new MovableSnotEye(targetPoint: new SnotPoint(Builder, body.EyeBody.Position, null, null)
                {
                    Used = true
                }, snot: this, body: Physics.EyeBody)
                : new SnotEye(this, Physics.EyeBody);
            eyeJointDef = new RevoluteJointDef(Physics.EyeJoint);
            Game.AddPositionProvider(new PositionProviderValue(this, 1f));
            StickyJoint = null;
            if (Game.BlackSide || Game.BonusChapter)
            {
                CreateTail();
            }
            touchEndTime = -1f;
            dynamicDrag = Config.GetBool("dynamicDrag");
            if (Game.LevelIndex == 169)
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
                Container.RemoveFromParent();
                joinedBodyClip.Clip.RemoveFromParent();
                int layer = joinedBodyClip.Clip.Layer;
                _ = Builder.AddChild(joinedBodyClip.Clip, layer);
                _ = Builder.AddChild(Container, layer);
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
            if (this.touch != null || StickyJoint != null || !Enabled)
            {
                return false;
            }
            Vector2 source = Builder.TouchRootVec(touch);
            if (movable && Vector2.Distance(source, Body.Position) > Vector2.Distance(source, Physics.EyeBody.Position))
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
            return 0f - Vector2.Distance((Vector2)param, bodyClip.Body.Position);
        }

        private static bool LinkableReq(BodyClip clip, object param)
        {
            //IL_000d: Unknown result type (might be due to invalid IL or missing references)
            LinkableReqParams linkableReqParams = (LinkableReqParams)param;
            return clip.Body.BodyType != 0 && Vector2.Distance(linkableReqParams.Position, clip.Body.Position) < linkableReqParams.Distance && clip is ISnotLinked && (clip as ISnotLinked).SnotEnabled;
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

        public virtual void CreateHighlite(ContreJourDXGame game)
        {
            if (!game.BlackSide)
            {
                highlite = new Sprite(game.NewFriendChapter ? "newFriend/McSnotEndHighlite_7" : ClipIds.Common.McSnotEndHighlite);
                highliteChanger = new CosChanger(0.05f, 0.1f);
            }
        }

        public void Blink()
        {
            Schedule(Blink, Maths.Random(10f, 25f));
            if (StickyJoint == null)
            {
                blinking = true;
                Eye.Open = true;
                Schedule(EndBlink, Maths.Random(2f, 5f));
            }
        }

        public void EndBlink()
        {
            blinking = false;
            if (StickyJoint == null)
            {
                Eye.Open = false;
            }
        }

        public virtual void CreateTail()
        {
            blackTail = new BlackTail(textureFile: Game.BlackSide ? "snotTailTextureBlack" : "McTailTextureGreen", body: Physics.EndBody, builder: Builder);
            Builder.Add(blackTail, 3);
            blackTail.Width = 20f;
        }

        public override void AddClipsToStage()
        {
            CreateHighlite(Game);
            if (highlite != null)
            {
                Container.AddChild(highlite);
            }
            Container.AddChild(ClipContent);
            Container.AddChild(BaseEndClip);
            Container.AddChild(BaseClip);
            if (Eye != null)
            {
                Container.AddChild(Eye);
            }
            Builder.Add(Container, Layer());
        }

        public override SnotSprite CreateClip()
        {
            ContreJourDXGame contreJourGame = (ContreJourDXGame)Builder.Game;
            return contreJourGame.ChooseSide<Func<SnotSprite>>(
                () => new BlackSnotSprite(contreJourGame, this, StartWidth, CenterWidth, EndWidth),
                () => new WhiteSnotSprite(contreJourGame, this, StartWidth, CenterWidth, EndWidth),
                () => new SpringSnotSprite(contreJourGame, this, StartWidth, CenterWidth, EndWidth),
                () => new SpringSnotSprite(contreJourGame, this, StartWidth, CenterWidth, EndWidth),
                () => new GreenSnotSprite(contreJourGame, this, StartWidth, CenterWidth, EndWidth))();
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
            if (StickyJoint == null && ((touchEndTime > 0f && Game.TotalTime - touchEndTime < 0.3f) || Dragging))
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
                if (StickyJoint != null)
                {
                    touchEndTime = 0f;
                }
            }
            ((SpringSnotSprite)ClipContent).Active = Dragging || Joined || blinking;
            base.Update(time);
            if (highlite != null)
            {
                highlite.Position = BaseEndClip.Position;
                highlite.OpacityByte = (int)(80f + (highliteChanger.Value * 80f));
                highliteChanger.Update(time);
            }
            if (movable)
            {
                BaseClip.Position = Builder.ToPoint(snotEye.Body.Position);
            }
            Eye.Position = BaseClip.Position;
            if (blackTail != null)
            {
                blackTail.Update(time);
                blackTail.Moving = StickyJoint != null;
            }
            ClipContent.OpacityFloat = ClipContent.OpacityFloat.StepTo(Enabled ? 1f : 0.5f, 0.05f);
            BaseEndClip.Color = Color.White * ClipContent.OpacityFloat;
        }

        public override Vector2 EndPosition()
        {
            return StickyJoint == null ? base.EndPosition() : StickyJoint.WorldAnchorB;
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
            return ((ContreJourDXGame)Builder.Game).ChooseSide("McSnotEndBlack", "McSnotEndWhite", "McSnotEnd", "McSnotEnd", "McSnotEnd_6");
        }

        public override string BaseClipName()
        {
            return ((ContreJourDXGame)Builder.Game).ChooseSide("McSnotStartBlack", "McSnotStartWhite", "McSnotStart", "McSnotStart", "McSnotStart_6");
        }

        public virtual string[] OnSound()
        {
            return Sounds.LeapOn;
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
            return GetDragTarget(Builder.TouchRootVec(touch).ClampDistance(Physics.EyeBody.Position, MaxLength()));
        }

        public BodyAndPoint GetDragTarget(Vector2 position)
        {
            List<BodyClip> list = FarseerUtil.Query(param: new LinkableReqParams(JoinDistance(), position), world: Builder.World, center: position, radius: JoinDistance(), clipPredicate: LinkableReq);
            if (list.Count == 0)
            {
                return new BodyAndPoint(null, position);
            }
            BodyClip bodyClip = (BodyClip)Arrays.MaxItem(list, ClosestReq, position);
            return new BodyAndPoint(bodyClip.Body, bodyClip.Body.Position);
        }

        public void SetZ(int value)
        {
            if (joinedBodyClip == null)
            {
                Builder.ChangeChildLayer(Container, value);
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
            if (StickyJoint != null)
            {
                SoundManager.PlayRandomSound(Sounds.LeapOut, 0.5f);
                SetDamping(FreeDamping());
                if (Linked != null)
                {
                    Linked.SnotJoinedCount--;
                    Linked.DestroyEvent.RemoveListener(OnLinkedDestroy);
                    Linked = null;
                }
                if (StickyJoint.BodyA != null && StickyJoint.BodyB != null)
                {
                    Builder.World.RemoveJoint((Joint)(object)StickyJoint);
                }
                StickyJoint = null;
                Eye.Open = false;
                ApplyDisconnectForce();
                SetZ(Layer());
                ReleaseEvent.SendEvent();
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
            if (bodyClip is ISnotLinked linked)
            {
                Linked = linked;
                Linked.SnotJoinedCount++;
                Linked.DestroyEvent.AddListener(OnLinkedDestroy);
            }
            SoundManager.PlayRandomSound(OnSound(), 0.5f);
            SetDamping(JoinedDamping());
            Physics.EndBody.SetTransform(joinPoint, Physics.EndBody.Rotation);
            Physics.EndBody.LinearVelocity = Vector2.Zero;
            StickyJoint = FarseerUtil.CreateRevoluteJoint(World, Physics.EndBody, joinBody, joinPoint);
            Eye.Open = true;
            SetZ(Layer() + 1);
            ContreJourDXGame.FocusOnHero();
            LinkEvent.SendEvent();
        }

        public static bool BodyConnectedToStaticProcessed(Body body, ref List<Body> processed)
        {
            //IL_0001: Unknown result type (might be due to invalid IL or missing references)
            //IL_0007: Invalid comparison between Unknown and I4
            if ((int)body.BodyType != 2)
            {
                return true;
            }
            for (JointEdge val = body.JointList; val != null; val = val.Next)
            {
                if (!processed.Contains(val.Other))
                {
                    processed.Add(val.Other);
                    if (val.Other.UserData is SnotBodyClip snot)
                    {
                        if (snot.ConnectedToStatic(ref processed))
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
                Builder.World.RemoveJoint((Joint)(object)Physics.EyeJoint);
                Physics.EyeJoint = null;
                jointRemoved = true;
            }
        }

        public virtual void EndDrag()
        {
            if (Dragging)
            {
                touchEndTime = Game.TotalTime;
                SetDamping(FreeDamping());
                Physics.EndBody.BodyType = (BodyType)2;
                SetZ(Layer());
                touch = null;
                if (jointRemoved)
                {
                    jointRemoved = false;
                    Physics.EyeBody.LinearVelocity = new Vector2(0f, 0f);
                    Physics.EyeJoint = eyeJointDef.Create(Builder.World);
                }
            }
        }
    }
}
