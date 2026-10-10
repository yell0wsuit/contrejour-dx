using Mokus2D.Visual.Particles.Util;

namespace ContreJourDX.Gameplay
{
    public class SnowFall : BlackFall
    {
        public SnowFall()
            : base("common/McSnowParticle")
        {
            SpeedMult = 2.5f;
        }

        public SnowFall(string textureName)
            : base(textureName)
        {
        }

        protected override void InitParams()
        {
            base.InitParams();
            AngularSpeed = new RandomRange(0f, 0f);
            ParticlesScale = new RandomRange(1.75f, 0.75f);
        }
    }
}
