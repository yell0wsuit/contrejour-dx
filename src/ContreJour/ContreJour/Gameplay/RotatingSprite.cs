using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace ContreJour.Gameplay
{
    public class RotatingSprite : Sprite
    {
        public float Speed { get; set; }

        public RotatingSprite(string name)
            : base(name)
        {
        }

        public RotatingSprite(ITexture texture)
            : base(texture)
        {
        }

        public RotatingSprite(SpriteData data)
            : base(data)
        {
        }

        public override void Update(float time)
        {
            RotationDegrees += Speed * time;
        }
    }
}
