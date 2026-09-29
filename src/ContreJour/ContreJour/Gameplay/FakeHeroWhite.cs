using System.IO;

using ContreJourMono.ContreJour.Menu.LevelComplete;

using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay
{
    public class FakeHeroWhite : FakeHeroBlack
    {
        protected override Color TailColor => ContreJourConstants.WhiteTailColor;

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
