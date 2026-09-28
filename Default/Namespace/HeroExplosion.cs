using Microsoft.Xna.Framework;

using Mokus2D.Effects.Tweening;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class HeroExplosion
{
    protected Explosion explosion;

    protected HeroBodyClip bodyClip;

    protected ContreJourGame game;

    public void Explode(HeroBodyClip _bodyClip, ContreJourGame _game)
    {
        bodyClip = _bodyClip;
        bodyClip.Body.BodyType = 0;
        game = _game;
        Sequence sequence = bodyClip.Clip.Tweener.StartSequence();
        HeroEye eye = bodyClip.Eye;
        eye.SetDefaultView();
        eye.AnimationsAllowed = false;
        bodyClip.Clip.Scale = bodyClip.Clip.ScaleX;
        for (int i = 0; i < 15; i++)
        {
            Vector2 position = new(Maths.Random(-2f, 2f), Maths.Random(-2f, 2f));
            position += bodyClip.Clip.Position;
            sequence = TweeningExtensions.ScaleTo(scale: 1f + (i / 15f / 5f) + ((i % 2 != 0) ? 0.05f : (-0.05f)), tweenObject: sequence.Next(0.02f)).MoveTo(position);
        }
        _ = sequence.OnComplete(DoExplode);
    }

    private void DoExplode()
    {
        explosion = new Explosion(game.BlackSide ? "chapter2/McHeroSmokeBlack" : "common/McWhiteSmoke");
        if (game.BonusChapter)
        {
            explosion.Color = ContreJourConstants.GreenLightColor;
        }
        explosion.Position = bodyClip.Clip.Position;
        explosion.HorizontalPosition = new RandomRange(0f, 5f);
        explosion.VerticalPosition = new RandomRange(0f, 5f);
        explosion.CreateOnStartPosition(14);
        bodyClip.Builder.Add(explosion, 10);
        bodyClip.DoExplode();
    }
}
