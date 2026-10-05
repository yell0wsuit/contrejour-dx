using System.Numerics;

using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public sealed class ValentineFall : GravityParticleSystem
    {
        public ValentineFall(Vector2 worldSize)
            : base("newFriend/McParticle_7")
        {
            HorizontalPosition = new RandomRange(worldSize.X / 2f, worldSize.X / 2f);
            VerticalPosition = new RandomRange(worldSize.Y, 0f);
            Speed = new RandomRange(-1f, 0f);
            // Reflect the web's -70-degree angle and Y-down coordinates into DX.
            Angle = new RandomRange(70f, 15f);
            AngularSpeed = new RandomRange(0f, float.RadiansToDegrees(0.1f));
            ParticlesScale = new RandomRange(0.5f, 0.2f);
            StartOpacity = new RandomRange(0.5f, 0.2f) * 255f;
            BottomLeftBound = new Vector2(0f, -20f);
            TopRightBound = new Vector2(worldSize.X + 20f, worldSize.Y);
        }

        public override void InitParticle(GravityParticle gravityParticle)
        {
            base.InitParticle(gravityParticle);
            gravityParticle.Speed *= 20f * gravityParticle.Scale;
        }
    }
}
