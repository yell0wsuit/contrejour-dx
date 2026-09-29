using System.Numerics;

using ContreJour.Clips.chapter1;
using ContreJour.Gameplay.Eyes;

namespace ContreJour.Gameplay
{
    public class BackSnotEye : MonsterEye
    {
        public static readonly EyeAnimation[] BackSnotAnimations =
        [
            new("McBackSnotEyeBlink"),
            new("McBackSnotEyeBlinkOneTime")
        ];

        protected override EyeAnimation[] Animations => BackSnotAnimations;

        protected override float ViewRadius => 30f;

        public BackSnotEye(ContreJourGame game, bool visible, Vector2 position)
            : base(game, visible, position)
        {
            EyeStep = 1.5f;
        }

        public override void Update(float time)
        {
            base.Update(time);
            CurrentBackground.Position = CurrentEyeBall.Position * 0.4f;
        }

        protected override void CreateDefaultView()
        {
            Background = new McBackSnotEye
            {
                Test = true
            };
            EyeBallSprite = new McBackSnotEyeBall();
        }
    }
}
