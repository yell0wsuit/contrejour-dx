using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Data
{
    public class VisualState
    {
        public bool TransformationDirty { get; set; }

        private Matrix4x4 matrix = Matrix4x4.Identity;

        public ref Matrix4x4 Matrix => ref matrix;
        private Vector2 _spritesScaleFactor = Vector2.One;

        private Color _color = Color.White;

        public Vector2 SpritesScaleFactor
        {
            get => _spritesScaleFactor;
            set => _spritesScaleFactor = value;
        }

        public float ColorRatio { get; private set; }

        public float Opacity { get; set; } = 1f;

        public Color GetColor(bool premultiply)
        {
            return ColorUtil.AddOpacity(_color, Opacity, premultiply);
        }

        public Matrix4x4 GetCombinedScreenMatrix(Vector2 size)
        {
            return Matrix * MatrixCache.GetScreenMatrix(size);
        }

        public VisualState()
            : this(Vector2.One)
        {
        }

        public VisualState(Vector2 scaleFactor)
        {
            TransformationDirty = false;
            _spritesScaleFactor = scaleFactor;
        }

        public VisualState(VisualState parent)
        {
            TransformationDirty = false;
            _spritesScaleFactor = parent._spritesScaleFactor;
        }

        public void Refresh(VisualState parentState, ref Matrix4x4 matrix, float nodeOpacity, Color nodeColor, float colorRatio, bool ignoreParentOpacity, bool ignoreParentColor, bool ignoreParentTransformations)
        {
            Matrix = ignoreParentTransformations ? matrix : (matrix * parentState.Matrix);
            RefreshValues(parentState, nodeOpacity, nodeColor, colorRatio, ignoreParentOpacity, ignoreParentColor);
            TransformationDirty = true;
        }

        public void RefreshValues(VisualState parentState, float nodeOpacity, Color nodeColor, float colorRatio, bool ignoreParentOpacity, bool ignoreParentColor)
        {
            Opacity = ignoreParentOpacity ? nodeOpacity : (nodeOpacity * parentState.Opacity);
            if (!ignoreParentColor)
            {
                _color = nodeColor.Mult(parentState._color);
            }
            else
            {
                _color = nodeColor;
                ColorRatio = colorRatio;
            }
            _color.A = byte.MaxValue;
            _spritesScaleFactor = parentState._spritesScaleFactor;
        }
    }
}
