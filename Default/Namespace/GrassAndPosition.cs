using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class GrassAndPosition(Particle particle, Vector2 position)
{
    protected Particle particle = particle;

    protected Vector2 position = position;

    public Particle Particle => particle;

    public Vector2 Position
    {
        get => position;
        set => position = value;
    }
}
