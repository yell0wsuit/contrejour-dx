using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual.Data;
using Mokus2D.Visual.Drawing.Effects;
using Mokus2D.Visual.Drawing.Vertex;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Drawing
{
    public static class SpriteBatchUtil
    {
        public static void DrawQuad<T>(Quad<T> quad, ref T[] vertices, ref short[] indices, ref int currentVertex, ref int currentIndex) where T : struct, IVertex
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

        public static void EnsureSize<T>(ref T[] vertices, ref short[] indices, int currentVertex, int currentIndex, int additionalVertices, int additionalIndices) where T : struct, IVertex
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

        public static void DrawUserIndexedPrimitives<T>(GraphicsDevice device, Vector2 screenSize, ref SpriteBatchProperties properties, ISpriteBatchEffect currentEffect, Texture2D texture, T[] vertices, short[] indices, int verticesCount, int indicesCount) where T : struct, IVertex
        {
            Matrix matrix = MatrixCache.GetScreenMatrix(screenSize);
            DrawUserIndexedPrimitives(device, ref matrix, ref properties, currentEffect, texture, vertices, indices, verticesCount, indicesCount);
        }

        public static void DrawUserIndexedPrimitives<T>(GraphicsDevice device, ref Matrix matrix, ref SpriteBatchProperties properties, ISpriteBatchEffect currentEffect, Texture2D texture, T[] vertices, short[] indices, int verticesCount, int indicesCount) where T : struct, IVertex
        {
            if (verticesCount > 0)
            {
                PrepareDraw(device, ref matrix, ref properties, currentEffect, texture);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, verticesCount, indices, 0, indicesCount / 3);
                Mokus2DGame.Instance.PerformanceCounter.IncreaseDrawCalls(indicesCount / 3);
            }
        }

        private static void PrepareDraw(GraphicsDevice device, ref Matrix matrix, ref SpriteBatchProperties properties, ISpriteBatchEffect currentEffect, Texture2D texture)
        {
            device.BlendState = properties.Blend;
            device.SamplerStates[0] = properties.SamplerState;
            currentEffect.Apply(matrix, texture);
        }
    }
}
