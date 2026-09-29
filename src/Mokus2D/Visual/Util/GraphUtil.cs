using System;
using System.Collections.Generic;
using System.Globalization;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Util
{
    public static class GraphUtil
    {
        private static readonly int[] indexData;

        static GraphUtil()
        {
            indexData = new int[2048];
            for (int i = 0; i < 2048; i++)
            {
                indexData[i] = i;
            }
        }

        public static void DrawTriangleFan(VertexPositionColor[] vertices)
        {
            short[] indices = CreateTriangleFanIndices((short)vertices.Length);
            DrawTriangleList(vertices, indices);
        }

        public static void DrawTriangleStrip<T>(T[] vertices) where T : struct, IVertexType
        {
            if (vertices.Length >= 3)
            {
                Mokus2DGame.Device.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
            }
        }

        public static void DrawTriangleList(VertexPositionColor[] vertices, short[] indices)
        {
            Mokus2DGame.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3, VertexPositionColor.VertexDeclaration);
        }

        public static void DrawTriangleList<T>(T[] vertices) where T : struct, IVertexType
        {
            Mokus2DGame.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3, vertices[0].VertexDeclaration);
        }

        public static void DrawLineList<T>(T[] vertices) where T : struct, IVertexType
        {
            Mokus2DGame.Device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertices.Length / 2, VertexPositionColor.VertexDeclaration);
        }

        public static void DrawLineStrip<T>(GraphicsDevice device, T[] vertices) where T : struct, IVertexType
        {
            device.DrawUserPrimitives(PrimitiveType.LineStrip, vertices, 0, vertices.Length - 1, VertexPositionColor.VertexDeclaration);
        }

        public static Vector2 StringToVector(string source)
        {
            string[] array = source.Split([',']);
            return new Vector2((float)Convert.ToDouble(array[0], CultureInfo.InvariantCulture), (float)Convert.ToDouble(array[1], CultureInfo.InvariantCulture));
        }

        public static void DrawRectangle(GraphicsDevice device, float x, float y, float width, float height, Color color)
        {
            DrawLineStrip(device, new VertexPositionColor[5]
            {
                new(new Vector3(x, y, 0f), color),
                new(new Vector3(x + width - 1f, y, 0f), color),
                new(new Vector3(x + width - 1f, y + height - 1f, 0f), color),
                new(new Vector3(x, y + height - 1f, 0f), color),
                new(new Vector3(x, y, 0f), color)
            });
        }

        public static VertexPositionColor[] GetVertexPositionColor(List<Vector2> polygon, List<Color> colors, int lenght = -1)
        {
            if (lenght == -1)
            {
                lenght = polygon.Count;
            }
            VertexPositionColor[] array = new VertexPositionColor[lenght];
            for (int i = 0; i < lenght; i++)
            {
                array[i].Position = polygon[i].ToVector3();
                array[i].Color = colors[i];
            }
            return array;
        }

        public static VertexPositionColor[] GetVertexPositionColor(List<Vector2> polygon, Color color, int lenght = -1)
        {
            if (lenght == -1)
            {
                lenght = polygon.Count;
            }
            VertexPositionColor[] array = new VertexPositionColor[lenght];
            for (int i = 0; i < lenght; i++)
            {
                array[i].Position = polygon[i].ToVector3();
                array[i].Color = color;
            }
            return array;
        }

        public static void FillSegmentsIndices(short[] indices, int count)
        {
            for (short num = 0; num < count / 6; num++)
            {
                short num2 = (short)(num * 6);
                short num3 = indices[num2] = (short)(num2 / 3);
                indices[num2 + 1] = (short)(num3 + 1);
                indices[num2 + 2] = (short)(num3 + 2);
                indices[num2 + 3] = (short)(num3 + 1);
                indices[num2 + 4] = (short)(num3 + 2);
                indices[num2 + 5] = (short)(num3 + 3);
            }
        }

        public static short[] CreateTriangleFanIndices(short count)
        {
            short[] array = new short[((count - 3) * 3) + 3];
            for (short num = 0; num < count - 2; num++)
            {
                short num2 = (short)(num * 3);
                array[num2] = 0;
                array[num2 + 1] = (short)(num + 1);
                array[num2 + 2] = (short)(num + 2);
            }
            return array;
        }

        public static Vector2 GetRootScale(this Node node)
        {
            Vector2 one = Vector2.One;
            while (node != null)
            {
                one *= node.ScaleVec;
                node = node.Parent;
            }
            return one;
        }

        public static float GetRootOpacity(this Node node)
        {
            float num = 1f;
            while (node != null)
            {
                num *= node.OpacityFloat;
                node = node.Parent;
            }
            return num;
        }

        public static float GetRootRotationRadians(this Node node)
        {
            float num = 0f;
            while (node != null)
            {
                num += node.RotationRadians;
                node = node.Parent;
            }
            return num;
        }

        public static Vector2 GetOutVertex(Vector2 inVertex, Vector2 nextInVertex, Vector2 controlVertex, float width)
        {
            Vector2 vector = controlVertex - inVertex;
            float num = (float)Math.Atan2(vector.Y, vector.X);
            num -= (float)Math.PI / 2f;
            return nextInVertex + VectorUtil.ToVector(width, num);
        }

        public static void CreateBezierPoints(IList<Vector2> polygon, int segments, IList<Vector2> result)
        {
            for (int i = 0; i < polygon.Count - 1; i += 2)
            {
                int index = (i != polygon.Count - 2) ? (i + 2) : 0;
                GetBezierPoints(polygon[i], polygon[i + 1], polygon[index], segments, insertLast: false, i, result);
            }
        }

        public static void GetBezierPoints(Vector2 origin, Vector2 control, Vector2 destination, int segments, bool insertLast, int index, IList<Vector2> result)
        {
            float num = 0f;
            for (int i = 0; i < segments; i++)
            {
                float x = ((float)Math.Pow(1f - num, 2.0) * origin.X) + (2f * (1f - num) * num * control.X) + (num * num * destination.X);
                float y = ((float)Math.Pow(1f - num, 2.0) * origin.Y) + (2f * (1f - num) * num * control.Y) + (num * num * destination.Y);
                result[index + i] = new Vector2(x, y);
                num += 1f / segments;
            }
            if (insertLast)
            {
                result[index + segments] = destination;
            }
        }

        public static void CreateGradientBorderColors(VertexPositionColor[] vertices, Color inColor)
        {
            Color color = inColor;
            color.A = 0;
            for (int i = 0; i < vertices.Length / 6; i++)
            {
                int num = i * 6;
                vertices[num].Color = inColor;
                vertices[num + 1].Color = inColor;
                vertices[num + 2].Color = color;
                vertices[num + 3].Color = inColor;
                vertices[num + 4].Color = color;
                vertices[num + 5].Color = color;
            }
        }

        public static void CreateGradientBorder(List<Vector2> surface, float width, VertexPositionColor[] vertices)
        {
            Vector2 vector = surface[0];
            Vector2 vector2 = GetOutVertex(surface[^1], vector, surface[1], width);
            for (int i = 0; i < surface.Count; i++)
            {
                Vector2 vector3 = surface[(i + 1) % surface.Count];
                Vector2 controlVertex = surface[(i + 2) % surface.Count];
                Vector2 outVertex = GetOutVertex(vector, vector3, controlVertex, width);
                int num = i * 6;
                vertices[num].Position = vector.ToVector3();
                vertices[num + 1].Position = vector3.ToVector3();
                vertices[num + 2].Position = vector2.ToVector3();
                vertices[num + 3].Position = vector3.ToVector3();
                vertices[num + 4].Position = vector2.ToVector3();
                vertices[num + 5].Position = outVertex.ToVector3();
                vector = vector3;
                vector2 = outVertex;
            }
        }

        public static void CreateGradientColorsStripStartColorEndColor(List<Color> colors, Color startColor, Color endColor)
        {
            for (int i = 0; i < colors.Count; i++)
            {
                colors[i] = (i % 2 != 0) ? endColor : startColor;
            }
        }

        public static void SetColor(VertexPositionColor[] vertices, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }

        public static void SetColor(VertexPositionColorTexture[] vertices, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }

        public static void SetGradientColorsStrip(Color startColor, Color endColor, VertexPositionColor[] colors)
        {
            for (int i = 0; i < colors.Length; i += 2)
            {
                colors[i].Color = startColor;
                colors[i + 1].Color = endColor;
            }
        }

        public static void CreateGradientColorsList(int surfaceSize, Color startColor, Color endColor, VertexPositionColorTexture[] colors)
        {
            _ = colors.Length;
            _ = surfaceSize * 6;
            for (int i = 0; i < surfaceSize; i++)
            {
                int num = i * 6;
                colors[num].Color = startColor;
                colors[num + 1].Color = startColor;
                colors[num + 2].Color = endColor;
                colors[num + 3].Color = startColor;
                colors[num + 4].Color = endColor;
                colors[num + 5].Color = endColor;
            }
        }

        public static void CreateGradientColors(int start, int end, Color fromColor, Color toColor, Color outColor, VertexPositionColorTexture[] colors)
        {
            Color color = fromColor;
            ColorDiff colorSub = toColor.Sub(fromColor) * (1f / (end - (float)start));
            Color color2 = color.Add(colorSub);
            for (int i = start; i < end; i++)
            {
                int num = i * 6;
                colors[num].Color = color;
                colors[num + 1].Color = color2;
                colors[num + 2].Color = outColor;
                colors[num + 3].Color = color2;
                colors[num + 4].Color = outColor;
                colors[num + 5].Color = outColor;
                color = color2;
                color2 = color2.Add(colorSub);
            }
        }

        public static void CreateGradientColorsForPolygonsStartEndStartColorEndColorColorsVector(int start, int end, Color startColor, Color endColor, List<List<Color>> colors)
        {
            if (colors.Count < end)
            {
                colors.Resize(end);
            }
            Color color = startColor;
            ColorDiff colorSub = endColor.Sub(startColor) * (1f / (end - (float)start));
            Color color2 = color.Add(colorSub);
            for (int i = start; i < end; i++)
            {
                List<Color> list = colors[i];
                list.Resize(4);
                list[0] = color;
                list[1] = color2;
                list[2] = color2;
                list[3] = color;
                color = color2;
                color2 = color2.Add(colorSub);
            }
        }

        public static void CreateGradientColorsForPolygonsStartColorEndColorColorsVector(int polygonCount, Color startColor, Color endColor, List<List<Color>> colors)
        {
            CreateGradientColorsForPolygonsStartEndStartColorEndColorColorsVector(0, polygonCount, startColor, endColor, colors);
        }

        public static void GetCircleRadiusSegmentsResult(Vector2 center, float radius, int segments, ref VertexPositionColorTexture[] result)
        {
            float num = (float)Math.PI * 2f / segments;
            float num2 = 0f;
            for (int i = 0; i < segments; i++)
            {
                result[i].Position = new Vector3((radius * Maths.Cos(num2)) + center.X, (radius * Maths.Sin(num2)) + center.Y, 0f);
                num2 += num;
            }
        }

        public static void FillConvexColors(List<Vector2> polygon, List<Color> colors)
        {
            DrawTriangleFan(GetVertexPositionColor(polygon, colors));
        }

        public static void FillConvexColor(List<Vector2> polygon, Color color)
        {
            DrawTriangleFan(GetVertexPositionColor(polygon, color));
        }

        public static void FillTrianglesStripTextureCoordsTextureColor(List<Vector2> triangles, List<Vector2> textureCoords, Color color)
        {
            FillTrianglesTextureCoordsTextureLoopTypeColor(triangles, textureCoords, PrimitiveType.TriangleStrip, color);
        }

        public static void FillTrianglesList<T>(T[] vertices) where T : struct, IVertexType
        {
            if (vertices != null && vertices.Length != 0)
            {
                Mokus2DGame.Device.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);
            }
        }

        public static void FillTrianglesTextureCoordsTextureColor(List<Vector2> triangles, List<Vector2> textureCoords, Color color)
        {
            FillTrianglesTextureCoordsTextureLoopTypeColor(triangles, textureCoords, PrimitiveType.TriangleList, color);
        }

        public static void FillTrianglesStripColors(List<Vector2> triangles, List<Color> colors)
        {
            FillTrianglesColorsLoopType(triangles, colors);
        }

        public static void FillTrianglesStripTextureCoordsTexture(List<Vector2> triangles, List<Vector2> textureCoords)
        {
            FillTrianglesTextureCoordsTextureLoopType(triangles, textureCoords, PrimitiveType.TriangleStrip);
        }

        public static void FillTrianglesTextureCoordsTexture(List<Vector2> vertices, List<Vector2> textureCoords)
        {
            FillTrianglesTextureCoordsTextureLoopType(vertices, textureCoords, PrimitiveType.TriangleList);
        }

        public static void FillTrianglesTextureCoordsTextureLoopType(List<Vector2> vertices, List<Vector2> textureCoords, PrimitiveType loopType)
        {
            FillTrianglesTextureCoordsTextureLoopTypeColor(vertices, textureCoords, loopType, new Color(255, 255, 255, 255));
        }

        public static void FillTrianglesTextureCoordsTextureLoopTypeColor(List<Vector2> vertices, List<Vector2> textureCoords, PrimitiveType loopType, Color color)
        {
            VertexPositionColorTexture[] array = new VertexPositionColorTexture[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                array[i].Position = vertices[i].ToVector3();
                array[i].Color = color;
                array[i].TextureCoordinate = textureCoords[i];
            }
            int primitiveCount = (loopType == PrimitiveType.TriangleList) ? (vertices.Count / 3) : (vertices.Count - 2);
            Mokus2DGame.Device.DrawUserPrimitives(loopType, array, 0, primitiveCount);
        }

        public static void FillTrianglesColorsLoopType(List<Vector2> vertices, List<Color> colors)
        {
            VertexPositionColor[] vertexPositionColor = GetVertexPositionColor(vertices, colors);
            DrawTriangleStrip(vertexPositionColor);
        }

        public static void FillTrianglesColors(List<Vector2> vertices, List<Color> colors)
        {
            FillTrianglesColorsLoopType(vertices, colors);
        }

        public static void FillConvex(List<Vector2> polygon)
        {
            FillConvexColor(polygon, new Color(0, 0, 0, 255));
        }

        public static void FillPolygonsColorsIteratorSize(List<List<Vector2>> points, List<List<Color>> colors, int size)
        {
            foreach (List<Color> color in colors)
            {
                for (int i = 0; i < size; i++)
                {
                    _ = points[i];
                    FillConvexColors(points[i], color);
                }
            }
        }

        public static void FillPolygonsColorsSize(List<List<Vector2>> points, List<List<Color>> colors, int size)
        {
            for (int i = 0; i < size; i++)
            {
                List<Vector2> polygon = points[i];
                List<Color> colors2 = colors[i];
                FillConvexColors(polygon, colors2);
            }
        }

        public static void FillPolygonsColors(List<List<Vector2>> points, List<List<Color>> colors)
        {
            FillPolygonsColorsSize(points, colors, points.Count);
        }

        public static void CreateTextureCoordsVerticesStep(int size, VertexPositionColorTexture[] vertices, float step)
        {
            for (int i = 0; i < size; i++)
            {
                int num = i * 6;
                float num2 = i * step;
                float x = num2 + step;
                vertices[num].TextureCoordinate = new Vector2(num2, 0f);
                vertices[num + 1].TextureCoordinate = new Vector2(x, 0f);
                vertices[num + 2].TextureCoordinate = new Vector2(num2, 1f);
                vertices[num + 3].TextureCoordinate = new Vector2(num2, 1f);
                vertices[num + 4].TextureCoordinate = new Vector2(x, 1f);
                vertices[num + 5].TextureCoordinate = new Vector2(x, 0f);
                for (int j = 0; j < 6; j++)
                {
                    vertices[num + j].Color = Color.White;
                }
            }
        }

        public static void CreateTextureCoordsVertices(int size, VertexPositionColorTexture[] vertices)
        {
            CreateTextureCoordsVerticesStep(size, vertices, 1f);
        }

        public static void CreateBorderTextureCoordsTextureWidthVertices(List<Vector2> surface, float textureWidth, ref List<Vector2> vertices)
        {
            float num = 0f;
            for (int i = 0; i < surface.Count; i++)
            {
                Vector2 vector = surface[i];
                Vector2 vector2 = surface[(i + 1) % surface.Count];
                float num2 = ((vector2 - vector).Length() / textureWidth) + num;
                int num3 = i * 6;
                vertices[num3] = new Vector2(0f, 0f);
                vertices[num3 + 1] = new Vector2(1f, 0f);
                vertices[num3 + 2] = new Vector2(0f, 1f);
                vertices[num3 + 3] = new Vector2(0f, 1f);
                vertices[num3 + 4] = new Vector2(1f, 1f);
                vertices[num3 + 5] = new Vector2(1f, 0f);
                num = num2 - (int)num2;
            }
        }

        public static void CreateGradientBorderWidthVertices(IList<Vector2> surface, float width, VertexPositionColorTexture[] vertices)
        {
            Vector2 vector = surface[0];
            Vector2 vector2 = GetOutVertex(surface[^1], vector, surface[1], width);
            for (int i = 0; i < surface.Count; i++)
            {
                Vector2 vector3 = surface[(i + 1) % surface.Count];
                Vector2 controlVertex = surface[(i + 2) % surface.Count];
                Vector2 outVertex = GetOutVertex(vector, vector3, controlVertex, width);
                int num = i * 6;
                vertices[num].Position = vector.ToVector3();
                vertices[num + 1].Position = vector3.ToVector3();
                vertices[num + 2].Position = vector2.ToVector3();
                vertices[num + 3].Position = vector3.ToVector3();
                vertices[num + 4].Position = vector2.ToVector3();
                vertices[num + 5].Position = outVertex.ToVector3();
                vector = vector3;
                vector2 = outVertex;
            }
        }

        public static void FillTrianglesColor(List<Vector2> triangles, Color color)
        {
            FillTrianglesTrianglesSizeColorLoopType(triangles, triangles.Count, color);
        }

        public static void FillTrianglesStripColor(List<Vector2> triangles, Color color)
        {
            FillTrianglesTrianglesSizeColorLoopType(triangles, triangles.Count, color);
        }

        public static void FillTrianglesTrianglesSizeColor(List<Vector2> triangles, int trianglesSize, Color color)
        {
            FillTrianglesTrianglesSizeColorLoopType(triangles, trianglesSize, color);
        }

        public static void FillTrianglesTrianglesSizeColorLoopType(List<Vector2> triangles, int trianglesSize, Color color)
        {
            DrawTriangleStrip(GetVertexPositionColor(triangles, color, trianglesSize));
        }

        public static void FillTrianglesTrianglesSize(List<Vector2> triangles, int trianglesSize)
        {
            FillTrianglesTrianglesSizeColor(triangles, trianglesSize, new Color(0, 0, 0, 255));
        }
    }
}
