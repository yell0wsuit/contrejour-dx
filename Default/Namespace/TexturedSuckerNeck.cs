using Microsoft.Xna.Framework;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class TexturedSuckerNeck : SuckerNeckSprite
{
    private const float TEXTURE_STEP = 0.75f;

    public TexturedSuckerNeck(string textureName)
    {
        Texture = ClipFactory.GetTexture(textureName);
        base.NeckColor = Color.White;
    }

    public override void CreateVectors(int _allPointsSize)
    {
        base.CreateVectors(_allPointsSize);
        GraphUtil.CreateTextureCoordsVerticesStep(_allPointsSize / 2 - 1, vertices, 0.75f);
    }

    public override void Bounce()
    {
        base.LightBounce();
    }

    public override void DrawBorder()
    {
    }

    public override void DrawPolygons()
    {
        GraphUtil.FillTrianglesList(vertices);
    }
}
