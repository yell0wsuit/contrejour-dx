using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class JoinableSpringBodyClip : RotatableSpringBase
    {
        private readonly RotatorBodyClip rotator;

        private Vector2 relativeRotatorPosition;

        private readonly float relativeAngle;

        protected override bool IsMoving => rotator != null && rotator.Body.AngularVelocity != 0f;

        public JoinableSpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config)
            : base(builder, body, clip, config)
        {
            rotator = (RotatorBodyClip)FarseerUtil.Query(Builder.World, Body.Position, 1.6666666f, typeof(RotatorBodyClip));
            relativeRotatorPosition = rotator.Body.GetLocalPoint(Body.Position);
            relativeAngle = rotator.Body.Rotation - Body.Rotation;
        }

        private void FixPosition()
        {
            Vector2 worldPoint = rotator.Body.GetWorldPoint(relativeRotatorPosition);
            float num = rotator.Body.Rotation - relativeAngle;
            Body.SetTransform(worldPoint, num);
        }

        public override void Update(float time)
        {
            if (rotator != null)
            {
                FixPosition();
            }
            base.Update(time);
        }

        protected override void CreateShadow()
        {
        }
    }
}
