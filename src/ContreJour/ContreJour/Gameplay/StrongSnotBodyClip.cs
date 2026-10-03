using System;
using System.Numerics;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

using Mokus2D.Graphics;
using Mokus2D.Input;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class StrongSnotBodyClip : SnotBodyClip
    {
        private FixedMouseJoint dragJoint;

        private readonly float extremeSnotDistance;

        private readonly float maxSnotDistance;
        private float targetColor;

        private float timeToRelease;

        public float NormalDistance { get; }

        public new Vector2 Position => Physics.FirstBody.Position;

        public StrongSnotBodyClip(LevelBuilderBase builder, SnotData body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            NormalDistance = CurrentDistance();
            maxSnotDistance = Math.Max(NormalDistance * 1.25f, NormalDistance + (30f * Builder.EngineConfig.SizeMultiplier));
            extremeSnotDistance = NormalDistance * 2f;
            targetColor = 255f;
        }

        public override string[] OnSound()
        {
            return Sounds.RopeOn;
        }

        public override float JoinDistance()
        {
            return 1.3333334f;
        }

        public override void CreateTail()
        {
        }

        public override void CreateHighlite(ContreJourGame game)
        {
        }

        public override void InitSizes()
        {
            base.InitSizes();
            CenterWidth = 10f * Builder.EngineConfig.SizeMultiplier;
        }

        public void EnsureSpeedY(float value)
        {
            for (int i = 0; i < Physics.BodiesSize(); i++)
            {
                Body val = Physics.BodyAt(i);
                if (val.LinearVelocity.Y > value)
                {
                    val.LinearVelocity = new Vector2(val.LinearVelocity.X, value);
                }
            }
        }

        public override string BaseEndClipName()
        {
            return Game.NewFriendChapter ? "McStrongSnotEnd_7" : Game.ChooseSide("McStrongSnotEndBlack", "McStrongSnotEndWhite", "McStrongSnotEnd", "McStrongSnotEnd", "McSnotEnd_6");
        }

        public override SnotSprite CreateClip()
        {
            return new TextureSnotSprite((ContreJourGame)Builder.Game, this, StartWidth, CenterWidth, EndWidth);
        }

        public float CurrentDistance()
        {
            return (Physics.FirstBody.Position - Physics.EndBody.Position).Length();
        }

        public override float JoinedDamping()
        {
            return 0f;
        }

        protected override void UpdateDragBodyPosition(BodyAndPoint target, float time)
        {
            base.UpdateDragBodyPosition(target, time);
            Body val = Physics.EndBody;
            int num = Physics.BodiesSize() - 2;
            while (num >= 0)
            {
                Body val2 = Physics.BodyAt(num);
                if ((val.Position - val2.Position).Length() > Physics.Metrics.PartSize * 1.2f)
                {
                    val2.Position = val2.Position.ClampDistance(val.Position, Physics.Metrics.PartSize * 1.2f);
                    val = val2;
                    num--;
                    continue;
                }
                break;
            }
        }

        public override bool TouchBegan(Touch touch)
        {
            //IL_0025: Unknown result type (might be due to invalid IL or missing references)
            //IL_002f: Expected O, but got Unknown
            if (base.TouchBegan(touch))
            {
                dragJoint = new FixedMouseJoint(Physics.EndBody, Physics.EndBody.Position)
                {
                    MaxForce = 100f,
                    Frequency = 100f,
                    WorldAnchorB = GetDragTarget().Point
                };
                return true;
            }
            return false;
        }

        public override void EndDrag()
        {
            base.EndDrag();
            if (dragJoint != null)
            {
                Builder.World.RemoveJoint((Joint)(object)dragJoint);
                dragJoint = null;
            }
        }

        public override void Update(float time)
        {
            base.Update(time);
            float b = targetColor;
            float num = CurrentDistance();
            bool flag = CanRelease();
            if (StickyJoint != null && num > maxSnotDistance && flag)
            {
                targetColor = Maths.StepTo(targetColor, 0f, 20f);
                if (timeToRelease > 0.6f || num > extremeSnotDistance)
                {
                    ReleaseSnot();
                }
                else
                {
                    timeToRelease += time;
                }
            }
            else
            {
                float num2 = 0f;
                if (flag && num > NormalDistance)
                {
                    num2 = (num - NormalDistance) / NormalDistance * 200f;
                }
                targetColor = Maths.StepTo(targetColor, 255f - num2, 20f);
                timeToRelease = 0f;
            }
            if (Maths.FuzzyNotEquals(targetColor, b))
            {
                ((TextureSnotSprite)ClipContent).TextureColor = new Color(255, (int)targetColor, (int)targetColor);
            }
        }

        public bool CanRelease()
        {
            return StickyJoint != null && (Linked is not HeroBodyClip || ((HeroBodyClip)Linked).OnGround() || Linked.SnotJoinedCount > 1);
        }

        public override void SetDamping(float value)
        {
        }

        public override float DragDistanceMultiplier()
        {
            return 1f;
        }

        public override void ApplyDisconnectForce()
        {
        }
    }
}
