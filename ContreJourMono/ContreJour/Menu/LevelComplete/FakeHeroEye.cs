using ContreJour.Content;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;

namespace ContreJourMono.ContreJour.Menu.LevelComplete;

public class FakeHeroEye : RandomAnimationEye
{
    protected override float ViewRadius => 12f;

    protected override EyeAnimation[] Animations => [];

    public FakeHeroEye()
        : base(null, useMask: true, new Vector2(80f, 80f))
    {
        AnimationsAllowed = false;
        EyeStep = 2f;
        UpdateEnabled = true;
    }

    public void Smile()
    {
        PlayAnimation(new EyeAnimation("McFakeHeroEyeSmile", null, lockY: true), force: true);
    }

    public void Open()
    {
        PlayAnimation(new EyeAnimation("McFakeHeroEyeOpen"), force: true);
    }

    public void Blink()
    {
        PlayAnimation(new EyeAnimation("McFakeHeroEyeBlink"), force: true);
    }

    protected override void CreateDefaultView()
    {
        string name = ProcessName("McFakeHeroEye");
        string name2 = ProcessName("McFakeHeroEyeBall");
        Background = (Sprite)ClipTypesCache.CreateNewNode(name);
        eyeBall = (Sprite)ClipTypesCache.CreateNewNode(name2);
    }

    public override void Update(float time)
    {
        base.Update(time);
        CurrentBackground.Position = CurrentEyeBall.Position * 0.5f;
    }
}
