using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class GravityParticleSystem : ParticleSystem
    {
        private Vector2 gravity;

        private RandomRange speed;

        private RandomRange angle;

        private RandomRange horizontalPosition;

        private RandomRange verticalPosition;

        private RandomRange angularSpeed;

        private RandomRange particlesScale = new(1f, 0f);

        private RandomRange startOpacity = new(255f, 0f);

        public Vector2 BottomLeftBound { get; set; } = new(float.NegativeInfinity, float.NegativeInfinity);

        public Vector2 TopRightBound { get; set; } = new(float.PositiveInfinity, float.PositiveInfinity);

        public RandomRange Speed
        {
            get => speed;
            set => speed = value;
        }

        public RandomRange Angle
        {
            get => angle;
            set => angle = value;
        }

        public RandomRange AngularSpeed
        {
            get => angularSpeed;
            set => angularSpeed = value;
        }

        public RandomRange ParticlesScale
        {
            get => particlesScale;
            set => particlesScale = value;
        }

        public RandomRange HorizontalPosition
        {
            get => horizontalPosition;
            set => horizontalPosition = value;
        }

        public RandomRange VerticalPosition
        {
            get => verticalPosition;
            set => verticalPosition = value;
        }

        public RandomRange StartOpacity
        {
            get => startOpacity;
            set => startOpacity = value;
        }

        public Vector2 Gravity
        {
            get => gravity;
            set => gravity = value;
        }

        public GravityParticleSystem(string textureName)
            : base(textureName)
        {
        }

        public GravityParticleSystem(IMovieClipData config, int count)
            : base(config, count)
        {
        }

        public GravityParticleSystem(IMovieClipData config)
            : base(config)
        {
        }

        public GravityParticleSystem(string textureName, int count)
            : base(textureName, count)
        {
        }

        public GravityParticleSystem(ISpriteData config)
            : base(config)
        {
        }

        public GravityParticleSystem(ISpriteData config, int count)
            : base(config, count)
        {
        }

        protected override void OnShowParticle(Particle particle)
        {
            InitParticle((GravityParticle)particle);
        }

        public virtual void InitParticle(GravityParticle gravityParticle)
        {
            float valueInRange = speed.GetValueInRange();
            float f = XnaMath.ToRadians(angle.GetValueInRange());
            gravityParticle.Speed = new Vector2(Maths.Cos(f) * valueInRange, Maths.Sin(f) * valueInRange);
            gravityParticle.OpacityByte = (int)startOpacity.GetValueInRange();
            gravityParticle.AngularSpeed = angularSpeed.GetValueInRange();
            gravityParticle.Scale = particlesScale.GetValueInRange();
            gravityParticle.Position = new Vector2(horizontalPosition.GetValueInRange(), verticalPosition.GetValueInRange());
        }

        public void CreateOnStartPosition(int count)
        {
            while (Particles.Count < count)
            {
                _ = AddParticle(new Vector2(horizontalPosition.GetValueInRange(), verticalPosition.GetValueInRange()));
            }
        }

        public void CreateBetweenBounds(int count)
        {
            while (Particles.Count < count)
            {
                _ = AddParticle(new Vector2(Maths.Random(BottomLeftBound.X, TopRightBound.X), Maths.Random(BottomLeftBound.Y, TopRightBound.Y)));
            }
        }

        public override Particle CreateParticle()
        {
            GravityParticle gravityParticle = Data is IMovieClipData movieClipData ? new GravityParticle(this, movieClipData) : new GravityParticle(this, (ISpriteData)Data);
            InitParticle(gravityParticle);
            return gravityParticle;
        }

        public override void UpdateParticleTime(Particle particle, float time)
        {
            GravityParticle gravityParticle = (GravityParticle)particle;
            gravityParticle.Speed += gravity * time;
            Vector2 vector = gravityParticle.Speed * time;
            gravityParticle.Position += vector;
            gravityParticle.RotationDegrees += gravityParticle.AngularSpeed;
            base.UpdateParticleTime(particle, time);
            if (gravityParticle.Position.X > TopRightBound.X || gravityParticle.Position.Y > TopRightBound.Y || gravityParticle.Position.X < BottomLeftBound.X || gravityParticle.Position.Y < BottomLeftBound.Y)
            {
                InitParticle(gravityParticle);
            }
        }
    }
}
