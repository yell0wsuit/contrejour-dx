using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    // iOS draws a disc at the snot's base and outlines it and the end with a thin ring that fades
    // inward, and lights the rings and the neck border up while the snot is active. Windows Phone
    // dropped the circles and never refreshed the border when the snot became active.
    public class SpringSnotSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : SnotSprite(snot, startWidth, centerWidth, endWidth)
    {
        private const int BaseCircleSegments = 16;

        private const int EndCircleSegments = 12;

        private readonly ContreJourGame game = game;

        private readonly Vector2[] baseCircleSurface = new Vector2[BaseCircleSegments];

        private readonly Vector2[] endCircleSurface = new Vector2[EndCircleSegments];

        private readonly Vertex[] baseCircle = new Vertex[BaseCircleSegments * 6];

        private readonly Vertex[] endCircle = new Vertex[EndCircleSegments * 6];

        private readonly Vertex[] baseDisc = new Vertex[(BaseCircleSegments - 2) * 3];

        private Color circlesNeckColor;

        protected float ActiveProgress { get; set; }

        private float previousActiveProgress = 1f;

        public bool Active { get; set; }

        public override void Update(float time)
        {
            base.Update(time);
            ActiveProgress = Maths.StepTo(ActiveProgress, Active ? 1 : 0, 0.05f);
            if (Maths.FuzzyNotEquals(ActiveProgress, previousActiveProgress))
            {
                SetCirclesColors();
                previousActiveProgress = ActiveProgress;
                if (Border != null)
                {
                    SetBorderColors();
                }
            }
            float startRadius = StartWidth / (1f / 30f) * 0.5f;
            float endRadius = (EndWidth / (1f / 30f) * 0.5f) + 1f;
            GraphUtil.GetCircle(Box2DConfig.DefaultConfig.ToPoint(Data.GetWorldStartPoint()), startRadius, baseCircleSurface);
            GraphUtil.GetCircle(Box2DConfig.DefaultConfig.ToPoint(Snot.EndPosition()), endRadius, endCircleSurface);
            GraphUtil.CreateGradientBorderWidthVertices(baseCircleSurface, -BorderWidth, baseCircle);
            GraphUtil.CreateGradientBorderWidthVertices(endCircleSurface, -BorderWidth, endCircle);
            GraphUtil.CreateConvexTriangles(baseCircleSurface, baseDisc);
        }

        public virtual Color BaseCircleColor()
        {
            return DrawNeckColor;
        }

        public virtual Color EndCircleColor()
        {
            return DrawNeckColor;
        }

        protected override void DrawPrimitives()
        {
            DrawCircles();
            base.DrawPrimitives();
        }

        public virtual void DrawCircles()
        {
            // The neck color picks up the node's opacity only when it draws.
            if (circlesNeckColor != DrawNeckColor)
            {
                SetCirclesColors();
            }
            GraphUtil.DrawTriangleList(baseCircle);
            if (!game.BlackSide)
            {
                GraphUtil.DrawTriangleList(endCircle);
            }
            GraphUtil.DrawTriangleList(baseDisc);
        }

        protected void SetCirclesColors()
        {
            circlesNeckColor = DrawNeckColor;
            Color endColor = EndColor();
            GraphUtil.CreateGradientColorsList(BaseCircleSegments, BaseCircleColor(), endColor, baseCircle);
            GraphUtil.CreateGradientColorsList(EndCircleSegments, EndCircleColor(), endColor, endCircle);
            GraphUtil.SetColor(baseDisc, BaseCircleColor());
        }

        public override Color EndColor()
        {
            int num = (int)(200f * ActiveProgress);
            return new Color(num, num, num, 0);
        }
    }
}
