using System;
using System.Numerics;

using ContreJour.Clips;
using ContreJour.Gameplay.Eyes;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class PlanetSnotEye : PlanetEye
    {
        protected override EyeAnimation[] Animations => SnotAnimations;

        protected override float ViewRadius => 7f;

        public PlanetSnotEye(ContreJourGame game, bool visible, Vector2 position)
            : base(game, visible, position)
        {
            UpdateEnabled = true;
        }

        protected override void CreateDefaultView()
        {
            Background = new Sprite(ClipIds.Common.McEyeMonster);
            EyeBallSprite = new Sprite(ClipIds.Common.McEyeBallMonster);
        }

        protected override float MaxAngle()
        {
            return (float)Math.PI;
        }
    }
}
