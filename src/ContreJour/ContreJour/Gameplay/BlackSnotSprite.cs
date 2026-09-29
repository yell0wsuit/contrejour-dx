using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class BlackSnotSprite : SpringSnotSprite
    {
        private readonly Color DefaultEndColor = new(0, 254, 254, 255);

        private readonly Color StartColor = new(0, 94, 118, 255);

        public BlackSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth)
            : base(game, snot, startWidth, centerWidth, endWidth)
        {
            BorderWidth = 3f;
        }

        public static int PointsInNeckPart()
        {
            return 4;
        }

        public virtual Color InitialStartColor()
        {
            return StartColor;
        }

        public virtual Color InitialEndColor()
        {
            return DefaultEndColor;
        }

        public override Color BaseCircleColor()
        {
            return InitialStartColor();
        }

        public override Color EndCircleColor()
        {
            return InitialEndColor();
        }

        public override Color EndColor()
        {
            Color result = EndCircleColor().Mult(ActiveProgress);
            result.A = 0;
            return result;
        }

        public virtual Color GetIntermidiateColorLineSize(int index, int lineSize)
        {
            return Color.Lerp(InitialStartColor(), InitialEndColor(), index / (float)lineSize);
        }

        public override void CreateVectors(int allPointsSize)
        {
            base.CreateVectors(allPointsSize);
            int num = allPointsSize / 2;
            Color color = InitialStartColor();
            Color intermidiateColorLineSize = GetIntermidiateColorLineSize(1, num);
            for (int i = 0; i < num - 1; i++)
            {
                int num2 = i * 6;
                Vertices[num2].Color = color;
                Vertices[num2 + 1].Color = intermidiateColorLineSize;
                Vertices[num2 + 2].Color = color;
                Vertices[num2 + 3].Color = color;
                Vertices[num2 + 4].Color = intermidiateColorLineSize;
                Vertices[num2 + 5].Color = intermidiateColorLineSize;
                color = intermidiateColorLineSize;
                intermidiateColorLineSize = GetIntermidiateColorLineSize(i + 2, num);
            }
        }

        protected override void SetNeckColors()
        {
        }

        public override void SetBorderColors()
        {
            BlackDrawUtil.SetBorderColors(AllPointsSize, InitialStartColor(), InitialEndColor(), EndColor(), Border);
        }

        public override void DrawPolygons()
        {
            GraphUtil.FillTrianglesList(Vertices);
        }
    }
}
