using System.Numerics;

using ContreJourDX.Clips;
using ContreJourDX.Gameplay.Eyes;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
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

        public BackSnotEye(ContreJourDXGame game, bool visible, Vector2 position)
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
            Background = new Sprite(ClipIds.Chapter1.McBackSnotEye)
            {
                Test = true
            };
            EyeBallSprite = new Sprite(ClipIds.Chapter1.McBackSnotEyeBall);
        }
    }
}
