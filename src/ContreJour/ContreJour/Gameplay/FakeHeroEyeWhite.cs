using ContreJourMono.ContreJour.Menu.LevelComplete;

namespace ContreJour.Gameplay;

public class FakeHeroEyeWhite : FakeHeroEyeBlack
{
    protected virtual string EyeBall => "McFakeHeroEyeBallWhite";

    protected override string ProcessName(string name)
    {
        return name == "McFakeHeroEyeBall" ? EyeBall : base.ProcessName(name);
    }
}
