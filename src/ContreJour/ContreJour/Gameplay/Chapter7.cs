using System.Numerics;

using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay
{
    // Mango follows New Friend in DX; the original artwork keeps its chapter6 names.
    public class Chapter7(int index, MainMenu menu) : ChapterItem(index, menu)
    {
        protected override void CreateSprites()
        {
            ParticleSystem flies = new("chapter6/McGreenPlanetFly");
            Container.AddChild(flies);
            AddAlphaItem(flies);
            AddUpdating(new PlanetSurround(flies));
            Background = new Sprite("chapter6/McChapter6Background");
            BlurBackground = new Sprite("chapter6/McChapter6Background");
            Container.AddChild(Background);
            ParticleSystem energy = new("common/McEnergyBall") { Scale = 2f };
            Container.AddChild(energy);
            AddAlphaItem(energy);
            AddUpdating(new PlanetEnergy(energy, Vector2.Zero, new RandomRange(0.16f, 0.06f)));
            Container.AddChild(new Sprite("chapter6/McChapter6Foreground"));
        }
    }
}
