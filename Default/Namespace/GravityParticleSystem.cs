using Microsoft.Xna.Framework;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Particles.Util;

namespace Default.Namespace;

public class GravityParticleSystem : ParticleSystem
{
    protected Vector2 gravity;

    protected RandomRange speed = default(RandomRange);

    protected RandomRange angle = default(RandomRange);

    protected RandomRange horizontalPosition = default(RandomRange);

    protected RandomRange verticalPosition = default(RandomRange);

    protected RandomRange angularSpeed = default(RandomRange);

    private RandomRange particlesScale = new RandomRange(1f, 0f);

    protected RandomRange startOpacity = new RandomRange(255f, 0f);

    protected Vector2 bottomLeftBound = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

    protected Vector2 topRightBound = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

    public RandomRange Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }

    public RandomRange Angle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
        }
    }

    public RandomRange AngularSpeed
    {
        get
        {
            return angularSpeed;
        }
        set
        {
            angularSpeed = value;
        }
    }

    public RandomRange ParticlesScale
    {
        get
        {
            return particlesScale;
        }
        set
        {
            particlesScale = value;
        }
    }

    public RandomRange HorizontalPosition
    {
        get
        {
            return horizontalPosition;
        }
        set
        {
            horizontalPosition = value;
        }
    }

    public RandomRange VerticalPosition
    {
        get
        {
            return verticalPosition;
        }
        set
        {
            verticalPosition = value;
        }
    }

    public Vector2 TopRightBound
    {
        get
        {
            return topRightBound;
        }
        set
        {
            topRightBound = value;
        }
    }

    public Vector2 BottomLeftBound
    {
        get
        {
            return bottomLeftBound;
        }
        set
        {
            bottomLeftBound = value;
        }
    }

    public RandomRange StartOpacity
    {
        get
        {
            return startOpacity;
        }
        set
        {
            startOpacity = value;
        }
    }

    public Vector2 Gravity
    {
        get
        {
            return gravity;
        }
        set
        {
            gravity = value;
        }
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
        initParticle((GravityParticle)particle);
    }

    public virtual void initParticle(GravityParticle gravityParticle)
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
        while (base.Particles.Count < count)
        {
            AddParticle(new Vector2(horizontalPosition.GetValueInRange(), verticalPosition.GetValueInRange()));
        }
    }

    public void CreateBetweenBounds(int count)
    {
        while (base.Particles.Count < count)
        {
            AddParticle(new Vector2(Maths.Random(bottomLeftBound.X, topRightBound.X), Maths.Random(bottomLeftBound.Y, topRightBound.Y)));
        }
    }

    public override Particle CreateParticle()
    {
        GravityParticle gravityParticle = ((Data is IMovieClipData) ? new GravityParticle(this, (IMovieClipData)Data) : new GravityParticle(this, (ISpriteData)Data));
        initParticle(gravityParticle);
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
            initParticle(gravityParticle);
        }
    }
}
