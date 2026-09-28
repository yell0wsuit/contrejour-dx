using System;

using ContreJour.Clips.planets;

using ContreJourMono.ContreJour.Game.Eyes;

using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Default.Namespace;

public class PlanetEye : BackSnotEye, IVectorPositionProvider
{
    private const float MAX_TIMEOUT = 10f;

    private const float MIN_TIMEOUT = 3f;

    private const float MAX_ANGLE = (float)Math.PI / 12f;

    private const float MAX_OFFSET = 10f;

    private const float MAX_SPEED = 2f;

    protected Vector2 eyePosition;

    protected Vector2 speed;

    protected override EyeAnimation[] Animations =>
    [
        new("McPlanetEyeBlink"),
        new("McPlanetEyeBlinkOneTime")
    ];

    public override float EyeStep => 0.25f;

    protected override float ViewRadius => 25f;

    public Vector2 PositionVec => eyePosition;

    public PlanetEye(ContreJourGame _game, bool _visible, Vector2 position)
        : base(_game, _visible, position)
    {
        eyePosition = Vector2.Zero;
        _ = Mokus2DGame.LoadMovieClipData("planets/McPlanetEyeBlink");
        _ = Mokus2DGame.LoadMovieClipData("planets/McPlanetEyeBlinkOneTime");
        UpdateEnabled = true;
    }

    private string MaskName()
    {
        return null;
    }

    public override void Update(float time)
    {
        base.Update(time);
        eyePosition += speed * time;
    }

    protected override void ScheduleAnimation()
    {
        _ = this.Schedule(Maths.Random(3f, 10f), Animate);
    }

    protected override void ChangePositionProvider()
    {
        base.ChangePositionProvider();
        float module = Maths.Random(-10f, 10f);
        float num = Maths.Random(0f - MaxAngle(), MaxAngle()) - ((float)Math.PI / 4f);
        eyePosition = VectorUtil.ToVector(module, num);
        positionProvider = this;
        speed = VectorUtil.ToVector(Maths.Random(2f), num + (float)Math.PI);
    }

    protected virtual float MaxAngle()
    {
        return (float)Math.PI / 12f;
    }

    protected override void CreateDefaultView()
    {
        background = new McPlanetEye();
        eyeBall = new McPlanet1EyeBall
        {
            Scale = 1.15f
        };
    }
}
