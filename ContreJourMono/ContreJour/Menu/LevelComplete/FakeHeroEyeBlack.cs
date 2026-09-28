namespace ContreJourMono.ContreJour.Menu.LevelComplete;

public class FakeHeroEyeBlack : FakeHeroEye
{
    protected override string ProcessName(string name)
    {
        return base.ProcessName(name).Replace("Eyes", "Eyes/Black") + "Black";
    }
}
