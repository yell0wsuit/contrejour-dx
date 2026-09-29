using System;

using Microsoft.Xna.Framework;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class Satellite : IUpdatable, IRemovable
    {
        private float direction;

        public float SpeedValue { get; set; }

        public float AngleStep { get; set; }
        public Particle Clip { get; }

        protected ContreJourGame Game { get; set; }

        private Vector2 initialPosition;

        protected BodyClip Target { get; set; }

        protected virtual Vector2 TargetPosition => Target == null ? initialPosition : Game.Builder.ToIPadPoint(Target.Body.Position);

        public bool ShouldRemove { get; private set; }

        public Satellite(ContreJourGame game, Particle clip, BodyClip parent, float direction, Vector2 position)
        {
            Target = parent;
            initialPosition = position;
            Clip = clip;
            SpeedValue = Maths.Random(15f, 25f) * 2f;
            AngleStep = Maths.Random(0.18f, 0.28f);
            this.direction = direction;
            Clip.Position = position;
            if (game != null)
            {
                Game = game;
                Game.AddUpdatable(this);
            }
            ShouldRemove = false;
        }

        public virtual void Update(float time)
        {
            float num = Math.Min(time, 1f / 30f);
            Vector2 vector = TargetPosition - Clip.Position;
            direction = Maths.StepTo(target: Maths.Atan2(vector.Y, vector.X).SimplifyAngle(direction - (float)Math.PI), maxStep: AngleStep * num * 30f, value: direction);
            Vector2 vector2 = VectorUtil.ToVector(SpeedValue * num, direction);
            Clip.Position += vector2;
        }

        public void Remove()
        {
            Clip.Visible = false;
            ShouldRemove = true;
        }
    }
}
