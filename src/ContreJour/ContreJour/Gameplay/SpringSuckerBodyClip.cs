using System;

using ContreJour.Clips.chapter5;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using Microsoft.Xna.Framework;

using Mokus2D.Events;
using Mokus2D.Input;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class SpringSuckerBodyClip : SuckerBodyClip
    {
        public EventSender ContactEvent { get; } = new();

        private Vector2 parallel;

        private Vector2 normal;

        private readonly float JumpImpulse = 1f;

        private readonly float SnotJumpImpulse = 0.2f;

        private readonly float SpeedMult = 0.75f;

        private bool touched;

        public bool Autocreated { get; private set; }

        protected override float BounceVolume => 0.4f;

        protected override string BounceSound => "spring";

        private bool CanAutocreate => Touch == null && Config.GetBool("auto");

        public SpringSuckerBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            AutoCreate();
        }

        public override void Restart()
        {
            if (CanAutocreate)
            {
                if (touched)
                {
                    base.Restart();
                    AutoCreate();
                }
            }
            else
            {
                base.Restart();
            }
        }

        private void AutoCreate()
        {
            Autocreated = CanAutocreate;
            if (Autocreated)
            {
                GhostSprite.Visible = false;
                CreatePosition = Body.Position;
                CreatePosition -= new Vector2(MaxDistance, 0f);
                CreateBodies();
            }
        }

        protected override SuckerNeckSprite CreateNeck()
        {
            return new SuckerNeckSprite();
        }

        public override Node CreatePimpa()
        {
            return new McSuckerBody();
        }

        public static void CreateLegs()
        {
        }

        public override void CreateBodies()
        {
            base.CreateBodies();
            parallel = VectorUtil.ToVector(1f, BounceAngle);
            normal = parallel.Rotate90();
        }

        public override void StartDrag(Touch touch)
        {
            base.StartDrag(touch);
            touched = true;
        }

        public override void OnCollisionStartPoint(Body body2, Contact point)
        {
            base.OnCollisionStartPoint(body2, point);
            if (End != null && (point.FixtureA == MiddleFixture || point.FixtureB == MiddleFixture) && body2.UserData is HeroBodyClip heroBodyClip)
            {
                Vector2 linearVelocity = heroBodyClip.Body.LinearVelocity;
                float num = VectorUtil.Atan2(linearVelocity);
                linearVelocity = VectorUtil.ToVector(angle: (float)Math.PI + BounceAngle - (num - BounceAngle - (float)Math.PI), module: linearVelocity.Length());
                Vector2 vector = VectorUtil.VectorProjection(linearVelocity, parallel);
                linearVelocity = VectorUtil.VectorProjection(linearVelocity, normal);
                linearVelocity *= SpeedMult;
                linearVelocity += vector;
                heroBodyClip.Body.LinearVelocity = linearVelocity;
                float num2 = (VectorUtil.Atan2(Body.Position, body2.Position) - BounceAngle).SimplifyAngle(-(float)Math.PI);
                float num3 = JumpImpulse + (heroBodyClip.SnotJoinedCount * SnotJumpImpulse);
                if (num2 < 0f)
                {
                    num3 *= -1f;
                }
                body2.ApplyLinearImpulse(VectorUtil.Rotate(new Vector2(0f, num3), BounceAngle), body2.WorldCenter);
                Neck.Bounce();
                ContactEvent.SendEvent();
                PlayBounceSound();
            }
        }
    }
}
