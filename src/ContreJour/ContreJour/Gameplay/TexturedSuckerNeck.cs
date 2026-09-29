using Microsoft.Xna.Framework;

using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class TexturedSuckerNeck : SuckerNeckSprite
    {
        public TexturedSuckerNeck(string textureName)
        {
            Texture = ClipFactory.GetTexture(textureName);
            NeckColor = Color.White;
        }

        public override void CreateVectors(int allPointsSize)
        {
            base.CreateVectors(allPointsSize);
            GraphUtil.CreateTextureCoordsVerticesStep((allPointsSize / 2) - 1, Vertices, 0.75f);
        }

        public override void Bounce()
        {
            base.LightBounce();
        }

        public override void DrawBorder()
        {
        }

        public override void DrawPolygons()
        {
            GraphUtil.FillTrianglesList(Vertices);
        }
    }
}
