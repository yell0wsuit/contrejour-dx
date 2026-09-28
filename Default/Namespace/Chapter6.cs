using Microsoft.Xna.Framework;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class Chapter6 : ChapterItem
{
    public Chapter6(int _index, MainMenu _menu)
        : base(_index, _menu)
    {
    }

    protected override void CreateSprites()
    {
        ParticleSystem particleSystem = new ParticleSystem("planets/McGreenPlanetFly");
        container.AddChild(particleSystem);
        AddAlphaItem(particleSystem);
        AddUpdating(new PlanetSurround(particleSystem));
        background = new Sprite("McChapter6Background");
        blurBackground = new Sprite("McChapter6Background");
        container.AddChild(background);
        ParticleSystem particleSystem2 = new ParticleSystem("common/McEnergyBall");
        container.AddChild(particleSystem2);
        particleSystem2.Scale = 2f;
        alphaItems.Add(particleSystem2);
        AddUpdating(new PlanetEnergy(particleSystem2, Vector2.Zero, new RandomRange(0.16f, 0.06f)));
        Sprite node = new Sprite("McChapter6Foreground");
        container.AddChild(node);
    }
}
