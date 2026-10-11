using Mokus2D.Visual.Particles.Util;

namespace ContreJourDX.Gameplay
{
    public class SidePanelParticles : LastParticles
    {
        public SidePanelParticles()
        {
            ParticlesScale = new RandomRange(1.5f, 0.6f);
            SpeedMult = 10f;
        }
    }
}
