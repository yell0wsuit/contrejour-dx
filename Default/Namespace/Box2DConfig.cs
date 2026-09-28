using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class Box2DConfig
{
    private Vector2 gravity;

    private int velocityIterations;

    private int positionIterations;

    private float sizeMultiplier;

    private float density;

    private float restitution;

    private float friction;

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
            field ??= new Box2DConfig();
            return field;
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
