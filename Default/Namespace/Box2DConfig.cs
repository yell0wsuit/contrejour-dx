using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class Box2DConfig
{
    protected Vector2 gravity;

    protected int velocityIterations;

    protected int positionIterations;

    protected float sizeMultiplier;

    protected float density;

    protected float restitution;

    protected float friction;

    private static Box2DConfig defaultConfigValue;

    public Vector2 Gravity
    {
        get => gravity;
        set => gravity = value;
    }

    public int VelocityIterations
    {
        get => velocityIterations;
        set => velocityIterations = value;
    }

    public int PositionIterations
    {
        get => positionIterations;
        set => positionIterations = value;
    }

    public float SizeMultiplier
    {
        get => sizeMultiplier;
        set => sizeMultiplier = value;
    }

    public float Density
    {
        get => density;
        set => density = value;
    }

    public float Restitution
    {
        get => restitution;
        set => restitution = value;
    }

    public float Friction
    {
        get => friction;
        set => friction = value;
    }

    public static Box2DConfig DefaultConfig
    {
        get
        {
            defaultConfigValue ??= new Box2DConfig();
            return defaultConfigValue;
        }
    }

    public Box2DConfig()
    {
        gravity = new Vector2(0f, -10f);
        velocityIterations = 20;
        positionIterations = 20;
        sizeMultiplier = 1f / 30f;
        density = 0.3f;
        restitution = 0f;
        friction = 1f;
    }

    public Vector2 ToPoint(Vector2 vec)
    {
        return vec / sizeMultiplier;
    }

    public Vector3 ToPoint(Vector3 vec)
    {
        return vec / sizeMultiplier;
    }

    public Vector2 ToVec(Vector2 point)
    {
        return point * sizeMultiplier;
    }
}
