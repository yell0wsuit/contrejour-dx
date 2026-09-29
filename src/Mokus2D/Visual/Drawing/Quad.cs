using System;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing
{
    public class Quad : IQuad
    {
        private Vertex leftTop;

        public ref Vertex LeftTop => ref leftTop;

        private Vertex rightTop;

        public ref Vertex RightTop => ref rightTop;

        private Vertex leftBottom;

        public ref Vertex LeftBottom => ref leftBottom;

        private Vertex rightBottom;

        public ref Vertex RightBottom => ref rightBottom;

        private Rectangle _bounds;

        private Vector2 _rightBottom;

        public Rectangle Bounds => _bounds;

        public void Refresh(Color color, Rectangle textureRect, Vector2 textureSize, Matrix4x4 matrix, Vector2 anchorInPixels, Vector2 size)
        {
            RefreshColor(color);
            RefreshTextureRect(textureRect, textureSize);
            RefreshTransformation(matrix, anchorInPixels, size);
        }

        public void RefreshTransformation(Matrix4x4 matrix, Vector2 anchorInPixels, Vector2 size)
        {
            Vector2 initialPosition = -anchorInPixels;
            Vector2 initialPosition2 = size - anchorInPixels;
            Vector2 initialPosition3 = new(initialPosition2.X, initialPosition.Y);
            Vector2 initialPosition4 = new(initialPosition.X, initialPosition2.Y);
            SetVertexPosition(ref LeftTop, initialPosition, ref matrix, cleanBounds: true);
            SetVertexPosition(ref RightTop, initialPosition3, ref matrix, cleanBounds: false);
            SetVertexPosition(ref LeftBottom, initialPosition4, ref matrix, cleanBounds: false);
            SetVertexPosition(ref RightBottom, initialPosition2, ref matrix, cleanBounds: false);
            _bounds.Width = (int)(_rightBottom.X - _bounds.X + 1f);
            _bounds.Height = (int)(_rightBottom.Y - _bounds.Y + 1f);
        }

        public void SetPositions(Vector2 leftTop, Vector2 rightBottom)
        {
            LeftTop.Position = new Vector3(leftTop, 0f);
            RightBottom.Position = new Vector3(rightBottom, 0f);
            LeftBottom.Position = new Vector3(leftTop.X, rightBottom.Y, 0f);
            RightTop.Position = new Vector3(rightBottom.X, leftTop.Y, 0f);
            RefreshBounds();
        }

        public void RefreshBounds()
        {
            _bounds = default;
            _bounds.X = (int)Maths.Min(LeftTop.Position.X, RightTop.Position.X, LeftBottom.Position.X, RightBottom.Position.X);
            _bounds.Y = (int)Maths.Min(LeftTop.Position.Y, RightTop.Position.Y, LeftBottom.Position.Y, RightBottom.Position.Y);
            _bounds.Width = (int)Maths.Max(LeftTop.Position.X, RightTop.Position.X, LeftBottom.Position.X, RightBottom.Position.X) - _bounds.X + 1;
            _bounds.Height = (int)Maths.Max(LeftTop.Position.Y, RightTop.Position.Y, LeftBottom.Position.Y, RightBottom.Position.Y) - _bounds.Y + 1;
        }

        private void SetVertexPosition(ref Vertex vertex, Vector2 initialPosition, ref Matrix4x4 matrix, bool cleanBounds)
        {
            Vector2 result = XnaMath.Transform(initialPosition, matrix);
            vertex.Position = new Vector3(result, 0f);
            if (cleanBounds)
            {
                _bounds = new Rectangle((int)result.X, (int)result.Y, 1, 1);
                _rightBottom = result;
                return;
            }
            _bounds.X = (int)Math.Min(result.X, _bounds.X);
            _bounds.Y = (int)Math.Min(result.Y, _bounds.Y);
            _rightBottom.X = Math.Max(result.X, _rightBottom.X);
            _rightBottom.Y = Math.Max(result.Y, _rightBottom.Y);
        }

        public void RefreshTextureRect(Rectangle textureRect, Vector2 textureSize)
        {
            Vector2 textureCoordinate = new(textureRect.X / textureSize.X, textureRect.Y / textureSize.Y);
            Vector2 textureCoordinate2 = new(textureRect.Right / textureSize.X, textureRect.Bottom / textureSize.Y);
            LeftTop.TextureCoordinate = textureCoordinate;
            RightBottom.TextureCoordinate = textureCoordinate2;
            LeftBottom.TextureCoordinate = new Vector2(textureCoordinate.X, textureCoordinate2.Y);
            RightTop.TextureCoordinate = new Vector2(textureCoordinate2.X, textureCoordinate.Y);
        }

        public void RefreshColor(Color color)
        {
            LeftTop.Color = color;
            RightTop.Color = color;
            LeftBottom.Color = color;
            RightBottom.Color = color;
        }

        public void Draw(IDrawer root)
        {
            root.Draw(this);
        }
    }
}
