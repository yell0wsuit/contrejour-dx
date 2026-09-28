using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Animation;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Interfaces;
using Mokus2D.Visual.Primitives.Collections;

namespace Mokus2D.Visual.Primitives;

public static class PrimitivesUtil
{
    public static void LineToPairs(List<Vector2> line, List<Pair<Vector2>> pairs, float width)
    {
        for (int i = 0; i < line.Count; i++)
        {
            Vector2 start = (i == 0) ? line[0] : line[i - 1];
            Vector2 end = (i == line.Count - 1) ? line[i] : line[i + 1];
            Pair<Vector2> orthoPoints = VectorUtil.GetOrthoPoints(line[i], start, end, width);
            pairs.Add(orthoPoints);
        }
    }

    public static void PairsLineToVertices<T>(List<Pair<Vector2>> line, T[] result) where T : IVertex
    {
        for (int i = 0; i < line.Count; i++)
        {
            ref readonly T reference = ref result[i * 2];
            Vector3 position = line[i].First.ToVector3();
            reference.Position = position;
            ref readonly T reference2 = ref result[(i * 2) + 1];
            Vector3 position2 = line[i].Second.ToVector3();
            reference2.Position = position2;
        }
    }

    public static void FillLineTexture<T>(T[] vertices, int count, ISpriteData spriteData) where T : IVertex
    {
        Vector2 vector = spriteData.TextureRect.LeftTop() / spriteData.Texture.Size();
        Vector2 vector2 = (spriteData.TextureRect.RightTop() - spriteData.TextureRect.LeftTop()) / (count / 2) / spriteData.Texture.Size();
        Vector2 vector3 = spriteData.TextureRect.LeftBottom() / spriteData.Texture.Size();
        for (int i = 0; i < count / 2; i++)
        {
            ref readonly T reference = ref vertices[i * 2];
            Vector2 textureCoordinate = vector + (vector2 * i);
            reference.TextureCoordinate = textureCoordinate;
            ref readonly T reference2 = ref vertices[(i * 2) + 1];
            Vector2 textureCoordinate2 = vector3 + (vector2 * i);
            reference2.TextureCoordinate = textureCoordinate2;
        }
    }

    public static void FillColor<T>(VerticesArray<T> vertices, Color color, float colorRatio) where T : struct, ITintVertex
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices.Items[i].Color = color;
            vertices.Items[i].ColorRatio = colorRatio;
        }
    }
}
