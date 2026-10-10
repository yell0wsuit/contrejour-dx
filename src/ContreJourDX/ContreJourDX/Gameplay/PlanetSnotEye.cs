using System;
using System.Numerics;

using ContreJourDX.Clips;
using ContreJourDX.Gameplay.Eyes;

using Mokus2D.Visual;

namespace ContreJourDX.Gameplay
{
    public class PlanetSnotEye : PlanetEye
    {
        protected override EyeAnimation[] Animations => SnotAnimations;

        protected override float ViewRadius => 7f;

        public PlanetSnotEye(ContreJourDXGame game, bool visible, Vector2 position)
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
