using System.IO;

using ContreJourDX.Menu.LevelComplete;

using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public class FakeHeroWhite : FakeHeroBlack
    {
        protected override Color TailColor => ContreJourDXConstants.WhiteTailColor;

        protected override string ProcessName(string name)
        {
            return Path.Combine(
            [
                TextureFolder,
                name + "White"
            ]);
        }

        protected override FakeHeroEye CreateEye()
        {
            return new FakeHeroEyeWhite();
        }
    }
}
