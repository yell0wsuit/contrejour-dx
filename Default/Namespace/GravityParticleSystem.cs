using Microsoft.Xna.Framework;

using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class GravityParticleSystem : ParticleSystem
{
    protected Vector2 gravity;

    protected RandomRange speed;

    protected RandomRange angle;

    protected RandomRange horizontalPosition;

    protected RandomRange verticalPosition;

    protected RandomRange angularSpeed;

    private RandomRange particlesScale = new(1f, 0f);

    protected RandomRange startOpacity = new(255f, 0f);

    protected Vector2 bottomLeftBound = new(float.NegativeInfinity, float.NegativeInfinity);

    protected Vector2 topRightBound = new(float.PositiveInfinity, float.PositiveInfinity);

    public RandomRange Speed
    {
        get => speed;
        set => speed = value;
    }

    public RandomRange Angle
    {
        get => angle;
        set => angle = value;
    }

    public RandomRange AngularSpeed
    {
        get => angularSpeed;
        set => angularSpeed = value;
    }

    public RandomRange ParticlesScale
    {
        get => particlesScale;
        set => particlesScale = value;
    }

    public RandomRange HorizontalPosition
    {
        get => horizontalPosition;
        set => horizontalPosition = value;
    }

    public RandomRange VerticalPosition
    {
        get => verticalPosition;
        set => verticalPosition = value;
    }

    public Vector2 TopRightBound
    {
        get => topRightBound;
        set => topRightBound = value;
    }

    public Vector2 BottomLeftBound
    {
        get => bottomLeftBound;
        set => bottomLeftBound = value;
    }

    public RandomRange StartOpacity
    {
        get => startOpacity;
        set => startOpacity = value;
    }

    public Vector2 Gravity
    {
        get => gravity;
        set => gravity = value;
    }

    public GravityParticleSystem(string textureName)
        : base(textureName)
    {
    }

    public GravityParticleSystem(IMovieClipData config, int count)
        : base(config, count)
    {
    }

    public GravityParticleSystem(IMovieClipData config)
        : base(config)
    {
    }

    public GravityParticleSystem(string textureName, int count)
        : base(textureName, count)
    {
    }

    public GravityParticleSystem(ISpriteData config)
        : base(config)
    {
    }

    public GravityParticleSystem(ISpriteData config, int count)
        : base(config, count)
    {
    }

    protected override void OnShowParticle(Particle particle)
    {
        InitParticle((GravityParticle)particle);
    }

    public virtual void InitParticle(GravityParticle gravityParticle)
    {
        float valueInRange = speed.GetValueInRange();
        float f = MathHelper.ToRadians(angle.GetValueInRange());
        gravityParticle.Speed = new Vector2(Maths.Cos(f) * valueInRange, Maths.Sin(f) * valueInRange);
        gravityParticle.OpacityByte = (int)startOpacity.GetValueInRange();
        gravityParticle.AngularSpeed = angularSpeed.GetValueInRange();
        gravityParticle.Scale = particlesScale.GetValueInRange();
        gravityParticle.Position = new Vector2(horizontalPosition.GetValueInRange(), verticalPosition.GetValueInRange());
    }

    public void CreateOnStartPosition(int count)
    {
        while (Particles.Count < count)
        {
            _ = AddParticle(new Vector2(horizontalPosition.GetValueInRange(), verticalPosition.GetValueInRange()));
        }
    }

    public void CreateBetweenBounds(int count)
    {
        while (Particles.Count < count)
        {
            _ = AddParticle(new Vector2(Maths.Random(bottomLeftBound.X, topRightBound.X), Maths.Random(bottomLeftBound.Y, topRightBound.Y)));
        }
    }

    public override Particle CreateParticle()
    {
        GravityParticle gravityParticle = (Data is IMovieClipData) ? new GravityParticle(this, (IMovieClipData)Data) : new GravityParticle(this, (ISpriteData)Data);
        InitParticle(gravityParticle);
        return gravityParticle;
    }

    public override void UpdateParticleTime(Particle particle, float time)
    {
        GravityParticle gravityParticle = (GravityParticle)particle;
        gravityParticle.Speed += gravity * time;
        Vector2 vector = gravityParticle.Speed * time;
        gravityParticle.Position += vector;
        gravityParticle.RotationDegrees += gravityParticle.AngularSpeed;
        base.UpdateParticleTime(particle, time);
        if (gravityParticle.Position.X > topRightBound.X || gravityParticle.Position.Y > topRightBound.Y || gravityParticle.Position.X < bottomLeftBound.X || gravityParticle.Position.Y < bottomLeftBound.Y)
        {
            InitParticle(gravityParticle);
        }
    }
}
