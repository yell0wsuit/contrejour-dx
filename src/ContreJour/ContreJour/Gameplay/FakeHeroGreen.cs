using ContreJour.Menu.LevelComplete;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJour.Gameplay
{
    public class FakeHeroGreen : FakeHeroBlack
    {
        protected override Color TailColor => ContreJourConstants.GreenTail.ChangeAlpha(byte.MaxValue);

        protected override string ProcessName(string name)
        {
            return name + "_6";
        }

        protected override FakeHeroEye CreateEye()
        {
            return new FakeHeroEyeGreen();
        }
    }
}
