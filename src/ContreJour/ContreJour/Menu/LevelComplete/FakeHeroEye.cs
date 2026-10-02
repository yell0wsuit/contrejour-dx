using ContreJour.Content;

using ContreJour.Gameplay.Eyes;

using Mokus2D.Visual;

namespace ContreJour.Menu.LevelComplete
{
    public class FakeHeroEye : RandomAnimationEye
    {
        protected override float ViewRadius => 12f;

        protected override EyeAnimation[] Animations => [];

        public FakeHeroEye()
            : base(null)
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
            Background = (Sprite)ClipCatalog.Create(name);
            EyeBallSprite = (Sprite)ClipCatalog.Create(name2);
        }

        public override void Update(float time)
        {
            base.Update(time);
            CurrentBackground.Position = CurrentEyeBall.Position * 0.5f;
        }
    }
}
