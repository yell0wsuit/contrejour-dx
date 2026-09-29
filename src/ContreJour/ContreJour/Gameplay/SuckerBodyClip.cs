using System;

using ContreJour.Clips.chapter5;
using ContreJour.Clips.common;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Data;
using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Integration.Farseer.Util;
using Mokus2D.Sound;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class SuckerBodyClip : ContreJourBodyClip, IClickable, IVectorPositionProvider, IRestartable
    {
        private static readonly Vector2 MinBorderOffset = new(30f);

        public EventSender FinishDragEvent { get; } = new();

        public EventSender RemoveEvent { get; } = new();

        protected Node GhostSprite { get; }

        protected float BounceAngle { get; set; }

        private Vector2 bouncePosition;

        private readonly Bouncer bouncer;

        protected Vector2 CreatePosition { get; set; }

        private bool creating;

        protected SuckerEndBodyClip End { get; set; }

        private Body endBody;

        private readonly MonsterEye eye;

        private readonly SuckerNeckSprite ghostNeck;

        private readonly Node ghostPimpa;

        private readonly McRoundDragFrameView limit;

        protected float MaxDistance { get; set; }

        protected Fixture MiddleFixture { get; set; }

        protected SuckerNeckSprite Neck { get; set; }

        private readonly Node pimpa;

        private readonly Sprite pimpaHighlite;

        private Vector2 pimpaPosition;

        private bool pulled;

        protected Touch Touch { get; set; }

        protected virtual float BounceVolume => 0.3f;

        protected virtual string BounceSound => "landing1";

        public bool Dragging => Touch != null;

        public Vector2 TargetPosition
        {
            get
            {
                Vector2 position = Builder.TouchRootVec(Touch);
                Vector2 vector = Builder.ToVec(MinBorderOffset);
                RectangleFloat levelScreenPhysicsBounds = Game.LevelScreenPhysicsBounds;
                levelScreenPhysicsBounds.Extend(XnaMath.Divide(-vector, Game.GameRoot.Scale));
                return levelScreenPhysicsBounds.ClampToBounds(position).ClampDistance(Body.Position, MaxDistance);
            }
        }

        public bool DisableHeroFocus => true;

        public SuckerBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            //IL_0043: Unknown result type (might be due to invalid IL or missing references)
            //IL_006c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0076: Expected O, but got Unknown
            clip = new Node();
            Clip = clip;
            builder.Add(clip, 1);
            Body val = builder.World.CreateCircle(0.1f, ((Body)body).Position);
            val.SetSensor(value: true);
            builder.World.RemoveBody((Body)body);
            Body = val;
            bouncer = new Bouncer(4f, 7f, 3f);
            Config["noShadow"] = "true";
            float num = Config.GetFloat("Width");
            MaxDistance = num / 2f * Builder.SizeMult;
            GhostSprite = new Node
            {
                OpacityFloat = 0.5f
            };
            Clip.AddChild(GhostSprite);
            ghostNeck = CreateNeck();
            ghostNeck.Color = new Color(10f / 51f, 10f / 51f, 10f / 51f, 1f);
            GhostSprite.AddChild(ghostNeck);
            ghostPimpa = CreatePimpa();
            GhostSprite.AddChild(ghostPimpa);
            limit = new McRoundDragFrameView();
            Builder.Add(limit, -1);
            limit.Position = Builder.ToPoint(Body.Position);
            limit.Scale = num / 200f;
            Neck = CreateNeck();
            Clip.AddChild(Neck);
            Node node = new McSuckerHighlite();
            Clip.AddChild(node);
            Sprite node2 = new McSuckerStart();
            Clip.AddChild(node2);
            eye = new MonsterEye(Game, visible: false, Body.Position);
            Clip.AddChild(eye);
            eye.Visible = false;
            pimpa = new Node();
            Clip.AddChild(pimpa);
            pimpaHighlite = new McSuckerHighlite();
            pimpa.AddChild(pimpaHighlite);
            Node node3 = CreatePimpa();
            pimpa.AddChild(node3);
        }

        public override float TouchDistance(Vector2 touchPosition)
        {
            return endBody != null
                ? Math.Min(Vector2.Distance(touchPosition, Body.Position), Vector2.Distance(touchPosition, endBody.Position))
                : Vector2.Distance(touchPosition, Body.Position);
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
            return false;
        }

        public bool TouchBegan(Touch touch)
        {
            if (creating)
            {
                return false;
            }
            if (Touch == null)
            {
                Vector2 target = Builder.TouchRootVec(touch);
                if (Vector2.Distance(Body.Position, target) <= 2f || Vector2.Distance(endBody.Position, target) <= 2f)
                {
                    StartDrag(touch);
                    return true;
                }
            }
            return false;
        }

        public void TouchEnd(Touch touch)
        {
            FinishDrag();
        }

        public bool TouchMove(Touch touch)
        {
            return true;
        }

        public void TouchOut(Touch touch)
        {
        }

        public virtual void Restart()
        {
            if (End != null && Touch == null)
            {
                eye.Open = false;
                eye.Visible = false;
                DestroyBodies();
                GhostSprite.Visible = false;
            }
        }

        public virtual Node CreatePimpa()
        {
            return new McSuckerBodyStrong();
        }

        protected virtual SuckerNeckSprite CreateNeck()
        {
            return new TexturedSuckerNeck("McSuckerStrongSnotTexture");
        }

        private static void RedrawGhost()
        {
        }

        public override void Update(float time)
        {
            base.Update(time);
            bouncer.Update(time);
            bouncePosition = pimpaPosition + VectorUtil.ToVector(bouncer.Value, BounceAngle);
            if (pimpa.Position != bouncePosition)
            {
                pimpa.Position = VectorUtil.StepTo(pimpa.Position, bouncePosition, 1000f * time);
                Neck.Length = pimpa.Position.Length();
                Neck.RotationDegrees = XnaMath.ToDegrees(Maths.Atan2(pimpa.Position.Y, pimpa.Position.X));
            }
            limit.OpacityByte = (int)Maths.StepTo(limit.OpacityByte, (Touch != null) ? 200 : 80, time * 200f);
            pimpaHighlite.OpacityByte = (int)Maths.StepTo(pimpaHighlite.OpacityByte, (End != null) ? 255 : 0, time * 600f);
            if (Touch != null)
            {
                RefreshGhostPosition(time, TargetPosition);
            }
            eye.UpdateNode(time);
            Neck.Update(time);
        }

        public void RefreshGhostPosition(float time, Vector2 position)
        {
            Vector2 vector = Builder.ToPoint(position - Body.Position);
            ghostPimpa.Position = new Vector2(vector.Length(), 0f);
            ghostNeck.Length = ghostPimpa.Position.X;
            ghostNeck.UpdateNode(time);
            RedrawGhost();
            GhostSprite.RotationDegrees = XnaMath.ToDegrees(Maths.Atan2(vector.Y, vector.X));
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            if (body2.UserData is BodyClip bodyClip && bodyClip is HeroBodyClip && Vector2.Distance(FarseerUtil.GetWorldPoint(point), Body.Position) > 1.1666666f && pulled)
            {
                Neck.Bounce();
                PlayBounceSound();
            }
        }

        protected void PlayBounceSound()
        {
            SoundManager.PlaySound(BounceSound, BounceVolume);
        }

        public virtual void StartDrag(Touch touch)
        {
            if (!creating)
            {
                if (Touch != null)
                {
                    throw new InvalidOperationException("already dragging");
                }
                SoundManager.PlaySound("leapOn1");
                GhostSprite.Tweener.Stop();
                GhostSprite.Visible = true;
                Touch = touch;
            }
        }

        public void DestroyBodies()
        {
            pulled = false;
            End = null;
            Builder.World.RemoveBody(endBody);
            Body.DestroyFixture(MiddleFixture);
            Body.SetSensor(value: true);
            pimpaPosition = Vector2.Zero;
            bouncer.Start();
            bouncer.Start();
        }

        public virtual void CreateBodies()
        {
            //IL_0091: Unknown result type (might be due to invalid IL or missing references)
            //IL_0097: Expected O, but got Unknown
            SoundManager.PlaySound("vysovuvannja", 0.3f);
            creating = false;
            pulled = true;
            endBody = Builder.World.CreateCircle(0.1f, CreatePosition);
            float num = Vector2.Distance(CreatePosition, Body.Position);
            float angle = VectorUtil.Atan2(Body.Position, CreatePosition);
            PolygonShape shape = new(Builder.EngineConfig.Density);
            shape.SetAsBox(num / 2f, 0.1f, VectorUtil.Rotate(new Vector2(num / 2f, 0f), angle), angle);
            BounceAngle = angle;
            MiddleFixture = FarseerUtil.AddShape((Shape)(object)shape, Body, Builder.EngineConfig.Density);
            Body.SetSensor(value: false);
            End = new SuckerEndBodyClip(this, endBody);
            pimpaPosition = Builder.ToPoint(endBody.Position - Body.Position);
            bouncer.Start();
            Neck.LightBounce();
            eye.Open = true;
            eye.PositionProvider = this;
            Schedule(RefreshPositionProvider, Maths.Random(1.5f, 2.5f));
            FinishDragEvent.SendEvent();
        }

        private void RefreshPositionProvider()
        {
            eye.PositionProvider = null;
        }

        public void CancelDrag()
        {
            Touch = null;
        }

        public void FinishDrag()
        {
            if (creating)
            {
                return;
            }
            bool flag = pulled;
            if (pulled)
            {
                DestroyBodies();
            }
            if (Vector2.Distance(TargetPosition, Body.Position) > 1.5f)
            {
                CreatePosition = TargetPosition;
                RefreshGhostPosition(0f, CreatePosition);
                if (flag)
                {
                    creating = true;
                    Schedule(CreateBodies, 0.2f);
                }
                else
                {
                    CreateBodies();
                }
            }
            else
            {
                eye.Open = false;
                GhostSprite.Visible = false;
                if (flag)
                {
                    RemoveEvent.SendEvent();
                }
            }
            Touch = null;
        }
    }
}
