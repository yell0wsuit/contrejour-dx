using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Primitives
{
    public class ColorRectangle : PrimitivesNode
    {
        private Vector2 size;

        private bool sizeDirty;

        private readonly Vertex[] vertices = new Vertex[4];

        public Vector2 Size
        {
            get => size;
            set
            {
                if (size != value)
                {
                    size = value;
                    sizeDirty = true;
                }
            }
        }

        public override float OpacityFloat
        {
            set
            {
                if (value != OpacityFloat)
                {
                    base.OpacityFloat = value;
                    Color = Color.ChangeAlpha(OpacityFloat);
                }
            }
        }

        public override Color Color
        {
            set
            {
                if (Color != value)
                {
                    base.Color = value;
                    RefreshColors();
                }
            }
        }

        public ColorRectangle(Color color, Vector2 size)
        {
            Color = color;
            Size = size;
            RefreshColors();
        }

        private void RefreshSize()
        {
            float x = size.X;
            float y = size.Y;
            vertices[0].Position = new Vector3(Vector2.Zero, 0f);
            vertices[1].Position = new Vector3(new Vector2(x, 0f), 0f);
            vertices[2].Position = new Vector3(new Vector2(0f, y), 0f);
            vertices[3].Position = new Vector3(new Vector2(x, y), 0f);
            sizeDirty = false;
        }

        private void RefreshColors()
        {
            GraphUtil.SetColor(vertices, Color);
        }

        protected override void DrawPrimitives()
        {
            TryRefreshSize();
            if (size.X > 0f && size.Y > 0f && OpacityByte > 0)
            {
                GraphUtil.DrawTriangleStrip(vertices);
            }
        }

        private void TryRefreshSize()
        {
            if (sizeDirty)
            {
                RefreshSize();
            }
        }
    }
}
