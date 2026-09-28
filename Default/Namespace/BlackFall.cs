using Microsoft.Xna.Framework;

using Mokus2D;
using Mokus2D.Visual;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class BlackFall : GravityParticleSystem
{
    public float SpeedMult = 4f;

    public BlackFall()
        : base(Mokus2DGame.LoadMovieClipData("common/McFallParticle"))
    {
        initParams();
    }

    public BlackFall(string textureName)
        : base(textureName)
    {
        initParams();
    }

    protected virtual void initParams()
    {
        Vector2 w7FromIPhoneSize = ScreenConstants.W7FromIPhoneSize;
        base.HorizontalPosition = new RandomRange(w7FromIPhoneSize.X / 2f, w7FromIPhoneSize.X / 2f);
        base.VerticalPosition = new RandomRange(w7FromIPhoneSize.Y + 20f, 0f);
        base.Speed = new RandomRange(5f, 0f);
        base.Angle = new RandomRange(-70f, 15f);
        base.AngularSpeed = new RandomRange(0f, 10f);
        base.ParticlesScale = new RandomRange(1f, 0.6f);
        bottomLeftBound = new Vector2(-20f, -20f);
        topRightBound = new Vector2(w7FromIPhoneSize.X + 20f, w7FromIPhoneSize.Y + 20f);
    }

    public override void initParticle(GravityParticle gravityParticle)
    {
        base.initParticle(gravityParticle);
        float num = SpeedMult * (gravityParticle.Scale - (base.ParticlesScale.Value - base.ParticlesScale.Offset)) + 1f;
        gravityParticle.Speed *= num;
    }
}
