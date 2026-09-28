using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PinHole : PrimitivesNode
{
    protected VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[24];

    public static readonly Vector3 IN_OFFSET = new(-10f, -10f, 0f);

    public static readonly Vector3 OUT_OFFSET = new(80f, 80f, 0f);

    public PinHole(Vector2 size)
    {
        CreateFatRectOffsetStartColorEndColor(size, OUT_OFFSET, Color.Green, Color.Black);
        CreateFatRectOffsetStartColorEndColor(size, IN_OFFSET, Color.Green, new Color(0, 0, 0, 0));
    }

    public void CreateFatRectOffsetStartColorEndColor(Vector2 size, Vector3 offset, Color startColor, Color endColor)
    {
        Vector3 vector = new(size.X, size.Y, 0f);
        Vector3 vector2 = new(size.X, 0f, 0f);
        Vector3 vector3 = new(0f, size.Y, 0f);
        vertices[0].Position = new Vector3(0f, 0f, 0f);
        vertices[1].Position = vector2;
        vertices[2].Position = -offset;
        vertices[3].Position = vector2;
        vertices[4].Position = -offset;
        vertices[5].Position = vector2 + new Vector3(offset.X, 0f - offset.Y, 0f);
        vertices[6].Position = vector2;
        vertices[7].Position = vector;
        vertices[8].Position = vector2 + new Vector3(offset.X, 0f - offset.Y, 0f);
        vertices[9].Position = vector;
        vertices[10].Position = vector2 + new Vector3(offset.X, 0f - offset.Y, 0f);
        vertices[11].Position = vector + offset;
        vertices[12].Position = vector;
        vertices[13].Position = vector3;
        vertices[14].Position = vector + offset;
        vertices[15].Position = vector3;
        vertices[16].Position = vector + offset;
        vertices[17].Position = vector3 + new Vector3(0f - offset.X, offset.Y, 0f);
        vertices[18].Position = vector3;
        vertices[19].Position = Vector3.Zero;
        vertices[20].Position = vector3 + new Vector3(0f - offset.X, offset.Y, 0f);
        vertices[21].Position = Vector3.Zero;
        vertices[22].Position = vector3 + new Vector3(0f - offset.X, offset.Y, 0f);
        vertices[23].Position = -offset;
        for (int i = 0; i < 4; i++)
        {
            int num = i * 6;
            vertices[num].Color = startColor;
            vertices[num + 1].Color = startColor;
            vertices[num + 2].Color = endColor;
            vertices[num + 3].Color = startColor;
            vertices[num + 4].Color = endColor;
            vertices[num + 5].Color = endColor;
        }
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.FillTrianglesList(vertices);
    }
}
