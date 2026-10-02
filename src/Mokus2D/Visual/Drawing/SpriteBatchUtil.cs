using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Visual.Data;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Drawing
{
    public static class SpriteBatchUtil
    {
        public static void DrawQuad(Quad quad, ref Vertex[] vertices, ref short[] indices, ref int currentVertex, ref int currentIndex)
        {
            EnsureSize(ref vertices, ref indices, currentVertex, currentIndex, 4, 6);
            vertices[currentVertex] = quad.LeftTop;
            vertices[currentVertex + 1] = quad.RightTop;
            vertices[currentVertex + 2] = quad.LeftBottom;
            vertices[currentVertex + 3] = quad.RightBottom;
            indices[currentIndex] = (short)currentVertex;
            indices[currentIndex + 1] = (short)(currentVertex + 1);
            indices[currentIndex + 2] = (short)(currentVertex + 2);
            indices[currentIndex + 3] = (short)(currentVertex + 3);
            indices[currentIndex + 4] = (short)(currentVertex + 2);
            indices[currentIndex + 5] = (short)(currentVertex + 1);
            currentVertex += 4;
            currentIndex += 6;
        }

        public static void EnsureSize(ref Vertex[] vertices, ref short[] indices, int currentVertex, int currentIndex, int additionalVertices, int additionalIndices)
        {
            if (currentVertex + additionalVertices >= vertices.Length)
            {
                Array.Resize(ref vertices, vertices.Length * 2);
            }
            if (currentIndex + additionalIndices >= indices.Length)
            {
                Array.Resize(ref indices, indices.Length * 2);
            }
        }

        public static void DrawTriangles(Vector2 screenSize, ref SpriteBatchProperties properties, ITexture texture, Vertex[] vertices, short[] indices, int verticesCount, int indicesCount)
        {
            if (verticesCount > 0)
            {
                Matrix4x4 matrix = MatrixCache.GetScreenMatrix(screenSize);
                DrawState state = new(texture, properties.Blend, properties.Sampler, ColorMode.Sprite);
                Mokus2DGame.Renderer.DrawTriangles(vertices, verticesCount, indices, indicesCount, matrix, state);
                Mokus2DGame.Instance.PerformanceCounter.IncreaseDrawCalls(indicesCount / 3);
            }
        }
    }
}
