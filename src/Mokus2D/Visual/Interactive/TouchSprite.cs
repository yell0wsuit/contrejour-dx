using Mokus2D.Graphics;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Interactive
{
    public class TouchSprite : Sprite
    {
        public TouchSprite(ISpriteData data)
            : base(data)
        {
            Clickable = true;
        }

        public TouchSprite(string name)
            : base(name)
        {
            Clickable = true;
        }

        public TouchSprite(ITexture texture)
            : base(texture)
        {
            Clickable = true;
        }
    }
}
