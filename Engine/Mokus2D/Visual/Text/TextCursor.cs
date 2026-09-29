using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Text;

public class TextCursor : Sprite
{
    public TextCursor(string name)
        : base(name)
    {
    }

    public TextCursor(ISpriteData data)
        : base(data)
    {
    }

    public TextCursor(Texture2D texture, IQuad quad = null)
        : base(texture, quad)
    {
    }
}
