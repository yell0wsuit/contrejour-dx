using Mokus2D.Visual.Particles.Util;

namespace ContreJourDX.Gameplay
{
    public class GroundFall : GravityParticleSystem
    {
        public GroundFall(ContreJourDXGame game)
            : base(game.NewFriendChapter ? "newFriend/McGroundPart_7" : game.Choose("common/McGroundPart", "common/McGroundPartBlack", "chapter4/McGroundPartWhite", null, "chapter6/McGroundPart_6"))
        {
            Angle = new RandomRange(270f, 0f);
            Speed = new RandomRange(40f, 20f);
            AngularSpeed = new RandomRange(0f, 0f);
            ParticlesScale = new RandomRange(1f, 0.3f);
            StartOpacity = new RandomRange(200f, 50f);
        }

        public override void UpdateParticleTime(Particle particle, float time)
        {
            base.UpdateParticleTime(particle, time);
            particle.Scale -= time / 2f;
            if (particle.Scale <= 0f)
            {
                particle.Visible = false;
            }
        }
    }
}
