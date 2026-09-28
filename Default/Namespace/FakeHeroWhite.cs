using System.IO;
using ContreJourMono.ContreJour.Menu.LevelComplete;
using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class FakeHeroWhite : FakeHeroBlack
{
    protected override Color TailColor => ContreJourConstants.WHITE_TAIL_COLOR;

    protected override string ProcessName(string name)
    {
        return Path.Combine(new string[2]
        {
            FakeHero.TextureFolder,
            name + "White"
        });
    }

    protected override FakeHeroEye CreateEye()
    {
        return new FakeHeroEyeWhite();
    }
}
