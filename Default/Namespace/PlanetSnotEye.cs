using System;

using ContreJour.Clips.common;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class PlanetSnotEye : PlanetEye
{
    protected override EyeAnimation[] Animations => SNOT_ANIMATIONS;

    protected override float ViewRadius => 7f;

    public PlanetSnotEye(ContreJourGame _game, bool _visible, Vector2 position)
        : base(_game, _visible, position)
    {
        UpdateEnabled = true;
    }

    protected override void CreateDefaultView()
    {
        background = new McEyeMonster();
        eyeBall = new McEyeBallMonster();
    }

    protected override float MaxAngle()
    {
        return (float)Math.PI;
    }
}
