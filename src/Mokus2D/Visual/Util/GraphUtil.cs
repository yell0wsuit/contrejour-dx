using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Util
{
    public static class GraphUtil
    {
        // Index lists for the draw helpers, grown on demand and reused by every draw.
        private static short[] _listIndices = [];

        private static short[] _stripIndices = [];

        public static void DrawTriangleStrip(Vertex[] vertices)
        {
            if (vertices.Length >= 3)
            {
                int triangles = vertices.Length - 2;
                if (_stripIndices.Length < triangles * 3)
                {
                    _stripIndices = CreateStripIndices(triangles);
                }
                Draw(vertices, _stripIndices, triangles * 3);
            }
        }

        public static void DrawTriangleList(Vertex[] vertices)
        {
            int triangles = vertices.Length / 3;
            if (triangles > 0)
            {
                if (_listIndices.Length < triangles * 3)
                {
                    _listIndices = CreateListIndices(triangles * 3);
                }
                Draw(vertices, _listIndices, triangles * 3);
            }
        }

        private static void Draw(Vertex[] vertices, short[] indices, int indexCount)
        {
            Mokus2DGame.Renderer.DrawTriangles(vertices, vertices.Length, indices, indexCount, PrimitivesDrawing.CurrentMatrix, PrimitivesDrawing.CurrentState);
        }

        private static short[] CreateListIndices(int count)
        {
            short[] indices = new short[count];
            for (int i = 0; i < count; i++)
            {
                indices[i] = (short)i;
            }
            return indices;
        }

        // Triangle i of a strip, in the vertex order GL uses (it swaps the first two on odd
        // triangles to keep the winding), so colors interpolate exactly as a GL strip draw did.
        private static short[] CreateStripIndices(int triangles)
        {
            short[] indices = new short[triangles * 3];
            for (int i = 0; i < triangles; i++)
            {
                bool odd = (i & 1) != 0;
                indices[i * 3] = (short)(odd ? i + 1 : i);
                indices[(i * 3) + 1] = (short)(odd ? i : i + 1);
                indices[(i * 3) + 2] = (short)(i + 2);
            }
            return indices;
        }

        public static Vector2 StringToVector(string source)
        {
            string[] array = source.Split([',']);
            return new Vector2((float)Convert.ToDouble(array[0], CultureInfo.InvariantCulture), (float)Convert.ToDouble(array[1], CultureInfo.InvariantCulture));
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

        // Fills result with its length of points around center, counterclockwise from angle 0.
        public static void GetCircle(Vector2 center, float radius, Vector2[] result)
        {
            float step = (float)(Math.PI * 2.0 / result.Length);
            float angle = 0f;
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = center + (new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius);
                angle += step;
            }
        }

        // The triangle list of the fan a convex surface's points make from its first point.
        public static void CreateConvexTriangles(IList<Vector2> surface, Vertex[] vertices)
        {
            for (int i = 0; i < surface.Count - 2; i++)
            {
                int num = i * 3;
                vertices[num].Position = new Vector3(surface[0], 0f);
                vertices[num + 1].Position = new Vector3(surface[i + 1], 0f);
                vertices[num + 2].Position = new Vector3(surface[i + 2], 0f);
            }
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

        public static void CreateGradientBorderColors(Vertex[] vertices, Color inColor)
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

        public static void CreateGradientBorder(List<Vector2> surface, float width, Vertex[] vertices)
        {
            Vector2 vector = surface[0];
            Vector2 vector2 = GetOutVertex(surface[^1], vector, surface[1], width);
            for (int i = 0; i < surface.Count; i++)
            {
                Vector2 vector3 = surface[(i + 1) % surface.Count];
                Vector2 controlVertex = surface[(i + 2) % surface.Count];
                Vector2 outVertex = GetOutVertex(vector, vector3, controlVertex, width);
                int num = i * 6;
                vertices[num].Position = new Vector3(vector, 0f);
                vertices[num + 1].Position = new Vector3(vector3, 0f);
                vertices[num + 2].Position = new Vector3(vector2, 0f);
                vertices[num + 3].Position = new Vector3(vector3, 0f);
                vertices[num + 4].Position = new Vector3(vector2, 0f);
                vertices[num + 5].Position = new Vector3(outVertex, 0f);
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

        public static void SetColor(Vertex[] vertices, Color color)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].Color = color;
            }
        }

        public static void SetGradientColorsStrip(Color startColor, Color endColor, Vertex[] colors)
        {
            for (int i = 0; i < colors.Length; i += 2)
            {
                colors[i].Color = startColor;
                colors[i + 1].Color = endColor;
            }
        }

        public static void CreateGradientColorsList(int surfaceSize, Color startColor, Color endColor, Vertex[] colors)
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

        public static void CreateGradientColors(int start, int end, Color fromColor, Color toColor, Color outColor, Vertex[] colors)
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

        public static void FillTrianglesList(Vertex[] vertices)
        {
            if (vertices != null)
            {
                DrawTriangleList(vertices);
            }
        }

        public static void CreateTextureCoordsVerticesStep(int size, Vertex[] vertices, float step)
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

        public static void CreateGradientBorderWidthVertices(IList<Vector2> surface, float width, Vertex[] vertices)
        {
            Vector2 vector = surface[0];
            Vector2 vector2 = GetOutVertex(surface[^1], vector, surface[1], width);
            for (int i = 0; i < surface.Count; i++)
            {
                Vector2 vector3 = surface[(i + 1) % surface.Count];
                Vector2 controlVertex = surface[(i + 2) % surface.Count];
                Vector2 outVertex = GetOutVertex(vector, vector3, controlVertex, width);
                int num = i * 6;
                vertices[num].Position = new Vector3(vector, 0f);
                vertices[num + 1].Position = new Vector3(vector3, 0f);
                vertices[num + 2].Position = new Vector3(vector2, 0f);
                vertices[num + 3].Position = new Vector3(vector3, 0f);
                vertices[num + 4].Position = new Vector3(vector2, 0f);
                vertices[num + 5].Position = new Vector3(outVertex, 0f);
                vector = vector3;
                vector2 = outVertex;
            }
        }

    }
}
