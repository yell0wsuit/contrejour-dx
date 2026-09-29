using System;

using ContreJour.Clips.common;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

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
