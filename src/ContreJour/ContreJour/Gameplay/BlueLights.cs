using ContreJour.Config;

using Microsoft.Xna.Framework;

using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    public class BlueLights : GravityParticleSystem
    {
        public BlueLights()
            : base("common/McGroundPartBlack")
        {
            Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
            HorizontalPosition = new RandomRange(w7FromIPhoneSize.X / 2f, w7FromIPhoneSize.X / 2f);
            VerticalPosition = new RandomRange(0f, 0f);
            Speed = new RandomRange(0.5f, 0f);
            Angle = new RandomRange(90f, 15f);
            AngularSpeed = new RandomRange(0f, 0f);
            ParticlesScale = new RandomRange(1.3f, 1f);
            BottomLeftBound = new Vector2(0f, 0f);
            TopRightBound = new Vector2(w7FromIPhoneSize.X, w7FromIPhoneSize.Y);
        }

        public override void InitParticle(GravityParticle gravityParticle)
        {
            base.InitParticle(gravityParticle);
            gravityParticle.OpacityByte = (int)(40f + (20f * gravityParticle.Scale));
            float num = 20f * gravityParticle.Scale;
            gravityParticle.Speed *= num;
        }
    }
}
