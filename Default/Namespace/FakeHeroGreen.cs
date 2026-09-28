using ContreJourMono.ContreJour.Menu.LevelComplete;
using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace Default.Namespace;

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
