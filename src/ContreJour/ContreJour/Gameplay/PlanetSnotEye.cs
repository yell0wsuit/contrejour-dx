using System;
using System.Numerics;

using ContreJour.Clips.common;
using ContreJour.Gameplay.Eyes;

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
            Background = new McEyeMonster();
            EyeBallSprite = new McEyeBallMonster();
        }

        protected override float MaxAngle()
        {
            return (float)Math.PI;
        }
    }
}
