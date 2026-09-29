using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class BlackFall : GravityParticleSystem
    {
        public float SpeedMult { get; set; } = 4f;

        public BlackFall()
            : base(Mokus2DGame.LoadMovieClipData("common/McFallParticle"))
        {
            InitParams();
        }

        public BlackFall(string textureName)
            : base(textureName)
        {
            InitParams();
        }

        protected virtual void InitParams()
        {
            Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
            HorizontalPosition = new RandomRange(w7FromIPhoneSize.X / 2f, w7FromIPhoneSize.X / 2f);
            VerticalPosition = new RandomRange(w7FromIPhoneSize.Y + 20f, 0f);
            Speed = new RandomRange(5f, 0f);
            Angle = new RandomRange(-70f, 15f);
            AngularSpeed = new RandomRange(0f, 10f);
            ParticlesScale = new RandomRange(1f, 0.6f);
            BottomLeftBound = new Vector2(-20f, -20f);
            TopRightBound = new Vector2(w7FromIPhoneSize.X + 20f, w7FromIPhoneSize.Y + 20f);
        }

        public override void InitParticle(GravityParticle gravityParticle)
        {
            base.InitParticle(gravityParticle);
            float num = (SpeedMult * (gravityParticle.Scale - (ParticlesScale.Value - ParticlesScale.Offset))) + 1f;
            gravityParticle.Speed *= num;
        }
    }
}
