using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Data;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing
{
    public interface IQuad
    {
        Rectangle Bounds { get; }

        void RefreshTransformation(Matrix4x4 matrix, Vector2 anchorInPixels, Vector2 size);

        void SetPositions(Vector2 leftTop, Vector2 rightBottom);

        void RefreshTextureRect(Rectangle textureRect, Vector2 textureSize);

        void RefreshColor(Color color);

        void Draw(IDrawer root);

        void RefreshBounds();
    }
}
