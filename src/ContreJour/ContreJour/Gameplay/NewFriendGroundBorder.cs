using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    // cj.js Shader.shadePath strokes the smooth outline outside a black fill.
    // Its ten shrinking strokes get darker toward the edge, with a vertical
    // orange-to-black gradient. This mesh follows the live deformable surface.
    public sealed class NewFriendGroundBorder : PlasticineWideBorder
    {
        private const int CurveSteps = 8;
        private const int Passes = 10;
        private const float StrokeWidth = 36f;
        // Reflect the web game's Y-down surface and flower offsets into DX's
        // Y-up physics coordinates. Keep the existing collision bodies intact.
        internal const float SurfaceOffset = 1f / 12f;
        internal const float GrassOffset = SurfaceOffset + (19.8f / 30f);
        private readonly PlasticineItem first;
        private readonly ContreJourGame game;
        private readonly Vector2[] points;
        private readonly Vector2[] normals;
        private readonly Vertex[] strip;
        private readonly float dropOff;
        private readonly float levelHeight;

        public NewFriendGroundBorder(PlasticineItem first, ContreJourGame game)
        {
            this.first = first;
            this.game = game;
            levelHeight = game.Builder.LevelSize.Y;
            int count = 0;
            float top = float.NegativeInfinity;
            float bottom = float.PositiveInfinity;
            PlasticineItem item = first;
            do
            {
                Vector2 center = SurfacePoint(item, 0f);
                Vector2 control = SurfacePoint(item, item.GetRightSurfacePositionLocal().X);
                top = Math.Max(top, Math.Max(center.Y, control.Y));
                bottom = Math.Min(bottom, Math.Min(center.Y, control.Y));
                count++;
                item = item.NextItem;
            }
            while (item != first);
            points = new Vector2[count * CurveSteps];
            normals = new Vector2[points.Length];
            strip = new Vertex[(points.Length + 1) * 2];
            dropOff = DropOff(top, bottom);
        }

        internal static float DropOff(float top, float bottom)
        {
            return bottom < 0f ? 0f : top - Math.Min(25f, (top - bottom) * 0.25f);
        }

        internal static Color StrokeColor(int pass, float y, float dropOff, float height, float lightPower)
        {
            float width = StrokeWidth * (Passes - pass) / Passes;
            float intensity = Math.Clamp(MathF.Floor(width * 6.5f * lightPower) / 255f, 0f, 1f);
            float shade = 1f - MathF.Cos(intensity * MathF.PI / 2f);
            float gradient = Math.Clamp((y - dropOff + (height * 0.06f)) / (height * 0.12f), 0f, 1f);
            return new Color((byte)(MathF.Floor(254f * shade) * gradient), (byte)(MathF.Floor(127f * shade) * gradient), (byte)(MathF.Floor(40f * shade) * gradient));
        }

        protected override void DrawPrimitives()
        {
            ReadSurface();
            for (int pass = 0; pass < Passes; pass++)
            {
                float outer = StrokeWidth * (Passes - pass) / Passes / 2f;
                float inner = StrokeWidth * (Passes - pass - 1) / Passes / 2f;
                SetStrip(outer, inner, pass);
                GraphUtil.DrawTriangleStrip(strip);
            }
            // Cover the gap to the existing black interior mesh with the same
            // sampled curve, rather than the old straight highlight segments.
            SetStrip(0f, -25f, -1);
            GraphUtil.DrawTriangleStrip(strip);
        }

        private void ReadSurface()
        {
            int index = 0;
            PlasticineItem item = first;
            do
            {
                Vector2 start = SurfacePoint(item, 0f);
                Vector2 control = SurfacePoint(item, item.GetRightSurfacePositionLocal().X);
                Vector2 end = SurfacePoint(item.NextItem, 0f);
                for (int step = 0; step < CurveSteps; step++)
                {
                    float t = step / (float)CurveSteps;
                    points[index++] = ((1f - t) * (1f - t) * start) + (2f * (1f - t) * t * control) + (t * t * end);
                }
                item = item.NextItem;
            }
            while (item != first);
            float area = 0f;
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 a = points[i];
                Vector2 b = points[(i + 1) % points.Length];
                area += (a.X * b.Y) - (b.X * a.Y);
            }
            for (int i = 0; i < points.Length; i++)
            {
                Vector2 tangent = points[(i + 1) % points.Length] - points[(i + points.Length - 1) % points.Length];
                Vector2 normal = area < 0f ? new Vector2(-tangent.Y, tangent.X) : new Vector2(tangent.Y, -tangent.X);
                normals[i] = normal.LengthSquared() > 0.000001f ? Vector2.Normalize(normal) : Vector2.UnitY;
            }
        }

        private Vector2 SurfacePoint(PlasticineItem item, float x)
        {
            return game.Builder.ToPoint(item.Body.GetWorldPoint(new Vector2(x, SurfaceOffset)));
        }

        private void SetStrip(float outer, float inner, int pass)
        {
            for (int i = 0; i <= points.Length; i++)
            {
                int index = i % points.Length;
                Vector2 a = points[index] + (normals[index] * outer);
                Vector2 b = points[index] + (normals[index] * inner);
                strip[i * 2] = new Vertex(new Vector3(a, 0f), pass < 0 ? Color.Black : StrokeColor(pass, a.Y, dropOff, levelHeight, game.LightPower), Vector2.Zero);
                strip[(i * 2) + 1] = new Vertex(new Vector3(b, 0f), pass < 0 ? Color.Black : StrokeColor(pass, b.Y, dropOff, levelHeight, game.LightPower), Vector2.Zero);
            }
        }
    }
}
