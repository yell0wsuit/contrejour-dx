using Microsoft.Xna.Framework.Graphics;

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

        public TouchDragSprite(Texture2D texture)
            : base(texture)
        {
        }
    }
}
