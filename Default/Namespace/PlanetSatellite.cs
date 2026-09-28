using System;
using ContreJour.Clips.menu;
using Microsoft.Xna.Framework;
using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Default.Namespace;

public class PlanetSatellite : Node, IUpdatable
{
    private const float ROTATION_STEP = 20f;

    private const float RADIUS = 150f;

    protected Sprite satellite;

    protected CosChanger changer;

    public PlanetSatellite()
    {
        satellite = new McSatellite();
        changer = new CosChanger(0.03f, 0.035f);
        changer.MinValue = -150f;
        changer.MaxValue = 150f;
        AddChild(satellite);
    }

    public override void Update(float time)
    {
        changer.Update(time);
        satellite.Position = new Vector2(changer.Value, 0f);
        satellite.Scale = changer.GetValue(0.5f, 1f, changer.Progress - (float)Math.PI / 2f);
        satellite.OpacityFloat = satellite.Scale;
        base.RotationDegrees += 20f * time;
        int nodeLayer = ((!(satellite.Scale < 0.75f)) ? 1 : (-1));
        base.Parent.ChangeChildLayer(this, nodeLayer);
    }
}
