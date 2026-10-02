using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJour.Menu.LevelComplete
{
    public class FakeHeroBlack : FakeHero
    {
        private static readonly Color TailColorValue = 1746142.ToRGBColor();

        protected override Color TailColor => TailColorValue;

        protected override FakeHeroEye CreateEye()
        {
            return new FakeHeroEyeBlack();
        }

        protected override string ProcessName(string name)
        {
            return base.ProcessName(name) + "Black";
        }
    }
}
