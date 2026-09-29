using ContreJour.Clips.menu2;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Gameplay
{
    public class Chapter4(int index, MainMenu menu) : Chapter2(index, menu)
    {
        protected override void CreateSprites()
        {
            Background = new McPlanet4Background();
            BlurBackground = new McChapter4Blur();
            Container.AddChild(Background);
            CreateBouncingSprite("menu2/McPlanet4Spring0", 40, new Vector2(-41f, 49f), 1f).Step = Maths.Random(0.05f, 0.08f);
            CreateBouncingSprite("menu2/McPlanet4Spring1", -70, new Vector2(69f, 24f), 1f).Step = Maths.Random(0.05f, 0.08f);
            CreateSmoke();
        }

        public override string SmokeSprite()
        {
            return "common/McWhiteSmoke";
        }

        public override Vector2 SmokeCoords()
        {
            return new Vector2(0f, 32f);
        }

        protected override void CreateBackLight()
        {
        }
    }
}
