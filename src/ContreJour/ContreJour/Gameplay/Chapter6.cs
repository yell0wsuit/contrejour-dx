using Microsoft.Xna.Framework;

using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace ContreJour.Gameplay;

public class Chapter6(int index, MainMenu menu) : ChapterItem(index, menu)
{
    protected override void CreateSprites()
    {
        ParticleSystem particleSystem = new("planets/McGreenPlanetFly");
        Container.AddChild(particleSystem);
        AddAlphaItem(particleSystem);
        AddUpdating(new PlanetSurround(particleSystem));
        Background = new Sprite("McChapter6Background");
        BlurBackground = new Sprite("McChapter6Background");
        Container.AddChild(Background);
        ParticleSystem particleSystem2 = new("common/McEnergyBall");
        Container.AddChild(particleSystem2);
        particleSystem2.Scale = 2f;
        AlphaItems.Add(particleSystem2);
        AddUpdating(new PlanetEnergy(particleSystem2, Vector2.Zero, new RandomRange(0.16f, 0.06f)));
        Sprite node = new("McChapter6Foreground");
        Container.AddChild(node);
    }
}
