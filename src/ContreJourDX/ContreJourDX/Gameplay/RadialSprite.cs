using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual;
using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Interfaces;

namespace ContreJourDX.Gameplay
{
    // A sprite drawn only within a wedge that sweeps clockwise from the top of the image, Progress of the way round.
    public class RadialSprite(string name) : Sprite(name)
    {
        public float Progress
        {
            get => ((RadialQuad)Quad).Progress;
            set => ((RadialQuad)Quad).Progress = value;
        }

        protected override IQuad CreateQuad()
        {
            return new RadialQuad();
        }

        // Lays the sprite's quad out as usual, then draws it as up to five triangles fanned from its center. Taken
        // over the image as a unit square, the wedges split at its corners, so each edge is cut exactly.
        private sealed class RadialQuad : IQuad
        {
            private static readonly float[] CornerAngles = [MathF.PI / 4f, MathF.PI * 3f / 4f, MathF.PI * 5f / 4f, MathF.PI * 7f / 4f, MathF.Tau];

            private readonly Quad quad = new();

            private readonly Quad[] wedges = [new(), new(), new(), new(), new()];

            public float Progress { get; set; }

            public Rectangle Bounds => quad.Bounds;

            public void RefreshTransformation(Matrix4x4 matrix, Vector2 anchorInPixels, Vector2 size)
            {
                quad.RefreshTransformation(matrix, anchorInPixels, size);
            }

            public void SetPositions(Vector2 leftTop, Vector2 rightBottom)
            {
                quad.SetPositions(leftTop, rightBottom);
            }

            public void RefreshTextureRect(Rectangle textureRect, Vector2 textureSize)
            {
                quad.RefreshTextureRect(textureRect, textureSize);
            }

            public void RefreshColor(Color color)
            {
                quad.RefreshColor(color);
            }

            public void RefreshBounds()
            {
                quad.RefreshBounds();
            }

            public void Draw(IDrawer root)
            {
                float sweep = Math.Clamp(Progress, 0f, 1f) * MathF.Tau;
                Vertex center = At(new Vector2(0.5f, 0.5f));
                float from = 0f;
                int count = 0;
                foreach (float corner in CornerAngles)
                {
                    float to = Math.Min(corner, sweep);
                    if (to > from)
                    {
                        // The second triangle of a quad is RightBottom, LeftBottom, RightTop: with RightBottom on
                        // RightTop it has no area, and only the wedge is drawn.
                        Quad wedge = wedges[count++];
                        wedge.LeftTop = center;
                        wedge.RightTop = At(Edge(from));
                        wedge.LeftBottom = At(Edge(to));
                        wedge.RightBottom = wedge.RightTop;
                        wedge.RefreshBounds();
                        root.Draw(wedge);
                    }
                    from = corner;
                    if (corner >= sweep)
                    {
                        break;
                    }
                }
            }

            // Where the ray at this angle, clockwise from the top, leaves the unit square (y down, as in the image).
            private static Vector2 Edge(float angle)
            {
                Vector2 direction = new(MathF.Sin(angle), -MathF.Cos(angle));
                float reach = 0.5f / Math.Max(Math.Abs(direction.X), Math.Abs(direction.Y));
                return new Vector2(0.5f) + (direction * reach);
            }

            // The vertex at a point of the image, as a unit square, interpolated from the quad's corners.
            private Vertex At(Vector2 point)
            {
                return new Vertex(
                    quad.LeftTop.Position + ((quad.RightTop.Position - quad.LeftTop.Position) * point.X) + ((quad.LeftBottom.Position - quad.LeftTop.Position) * point.Y),
                    quad.LeftTop.Color,
                    quad.LeftTop.TextureCoordinate + ((quad.RightTop.TextureCoordinate - quad.LeftTop.TextureCoordinate) * point.X) + ((quad.LeftBottom.TextureCoordinate - quad.LeftTop.TextureCoordinate) * point.Y));
            }
        }
    }
}
