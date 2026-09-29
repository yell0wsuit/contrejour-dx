using Microsoft.Xna.Framework;

using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Visual.Drawing
{
    public interface IQuad
    {
        Rectangle Bounds { get; }

        void RefreshTransformation(Matrix matrix, Vector2 anchorInPixels, Vector2 size);

        void SetPositions(Vector2 leftTop, Vector2 rightBottom);

        void RefreshTextureRect(Rectangle textureRect, Vector2 textureSize);

        void RefreshColor(Color color, float colorAmount);

        void Draw(IDrawer root);

        void RefreshBounds();
    }
}
