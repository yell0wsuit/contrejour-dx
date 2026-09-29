using System;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Data;

public class SpriteData(string id) : TextureNodeData(id), ISpriteData, ITextureNodeData, IConfig, ICloneable<SpriteData>
{
    public FrameData Frame { get; set; }

    public Vector2 Size => TextureRect.Size() * ScaleFactor;

    public Vector2 Anchor => Frame.Anchor;

    public Rectangle TextureRect => Frame.Rect;

    public SpriteData Clone()
    {
        return (SpriteData)MemberwiseClone();
    }
}
