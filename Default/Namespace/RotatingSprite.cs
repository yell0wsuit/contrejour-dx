using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class RotatingSprite : Sprite
{
    public float Speed { get; set; }

    public RotatingSprite(string name)
        : base(name)
    {
    }

    public RotatingSprite(Texture2D texture)
        : base(texture)
    {
    }

    public RotatingSprite(SpriteData data)
        : base(data)
    {
    }

    public override void Update(float time)
    {
        base.RotationDegrees += Speed * time;
    }
}
