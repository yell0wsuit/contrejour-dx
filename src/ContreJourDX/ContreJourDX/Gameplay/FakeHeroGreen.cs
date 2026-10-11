using ContreJourDX.Menu.LevelComplete;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;

namespace ContreJourDX.Gameplay
{
    public class FakeHeroGreen : FakeHeroBlack
    {
        protected override Color TailColor => ContreJourDXConstants.GreenTail.ChangeAlpha(byte.MaxValue);

        protected override string ProcessName(string name)
        {
            return TextureFolder + "/" + name + "_6";
        }

        protected override FakeHeroEye CreateEye()
        {
            return new FakeHeroEyeGreen();
        }
    }
}
