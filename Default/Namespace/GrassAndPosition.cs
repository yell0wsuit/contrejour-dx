using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class GrassAndPosition
{
    protected Particle particle;

    protected Vector2 position;

    public Particle Particle => particle;

    public Vector2 Position
    {
        get => position;
        set => position = value;
    }

    public GrassAndPosition(Particle particle, Vector2 position)
    {
        this.particle = particle;
        this.position = position;
    }
}
