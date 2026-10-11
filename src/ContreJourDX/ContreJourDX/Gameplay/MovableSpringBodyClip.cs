using System.Numerics;

using FarseerPhysics.Dynamics;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class MovableSpringBodyClip(LevelBuilderBase builder, object body, Node clip, Hashtable config) : DynamicSpringBodyClip(builder, body, clip, config)
    {
        private DragableBodyClip mover;

        private Vector2 offset;

        private Vector2 moverPosition;

        public override void Update(float time)
        {
            if (mover == null)
            {
                mover = (DragableBodyClip)FarseerUtil.Query(Builder.World, Body.Position, 3.3333333f, typeof(DragableBodyClip));
                offset = Body.Position - mover.Body.Position;
                moverPosition = mover.Body.Position;
                Body.BodyType = (BodyType)1;
            }
            else if (moverPosition != mover.Body.Position)
            {
                Vector2 vector = mover.Body.Position + offset;
                Body.SetTransform(vector, Body.Rotation);
                moverPosition = mover.Body.Position;
            }
            base.Update(time);
        }

        protected override void CreateShadow()
        {
        }
    }
}
