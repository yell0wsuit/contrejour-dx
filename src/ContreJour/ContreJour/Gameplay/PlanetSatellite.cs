using System;
using System.Numerics;

using ContreJour.Clips;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class PlanetSatellite : Node, IUpdatable
    {
        private readonly Sprite satellite;

        private readonly CosChanger changer;

        public PlanetSatellite()
        {
            satellite = new Sprite(ClipIds.Menu.McSatellite);
            changer = new CosChanger(0.03f, 0.035f)
            {
                MinValue = -150f,
                MaxValue = 150f
            };
            AddChild(satellite);
        }

        public override void Update(float time)
        {
            changer.Update(time);
            satellite.Position = new Vector2(changer.Value, 0f);
            satellite.Scale = CosChanger.GetValue(0.5f, 1f, changer.Progress - ((float)Math.PI / 2f));
            satellite.OpacityFloat = satellite.Scale;
            RotationDegrees += 20f * time;
            int nodeLayer = (!(satellite.Scale < 0.75f)) ? 1 : (-1);
            Parent.ChangeChildLayer(this, nodeLayer);
        }
    }
}
