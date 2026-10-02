using Mokus2D.Graphics;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Interactive
{
    public class TouchDragSprite : TouchSprite
    {
        public TouchDragSprite(ISpriteData data)
            : base(data)
        {
        }

        public TouchDragSprite(string name)
            : base(name)
        {
        }

        public TouchDragSprite(ITexture texture)
            : base(texture)
        {
        }
    }
}
