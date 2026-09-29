using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class PlasticineBorder : PrimitivesNode, IOpacity
    {
        private readonly Vertex[] outBorder;

        private readonly Vertex[] inBorder;

        private readonly int polygonSize;

        private static readonly Color DefaultOutColor = new Color(255, 255, 255) * 0f;

        private static readonly Color DefaultInColor = new(0, 0, 0, 0);

        private static readonly Color DefaultCenterColor = new(127, 127, 127);

        private readonly float WIDTH = 4f;

        public override float OpacityFloat
        {
            set
            {
                if (OpacityFloat != value)
                {
                    base.OpacityFloat = value;
                    CreateColors();
                }
            }
        }

        public PlasticineBorder(List<Vector2> initialPolygon)
        {
            List<Vector2> surface = [];
            ContreDrawUtil.CreateBezierSurfaceSurfaceSegments(initialPolygon, ref surface, 3);
            polygonSize = surface.Count;
            outBorder = new Vertex[surface.Count * 6];
            inBorder = new Vertex[surface.Count * 6];
            GraphUtil.CreateGradientBorderWidthVertices(surface, BorderWidth(), inBorder);
            GraphUtil.CreateGradientBorderWidthVertices(surface, 0f - BorderWidth(), outBorder);
            CreateColors();
            OpacityFloat = 0f;
        }

        public virtual float BorderWidth()
        {
            return WIDTH / 2f;
        }

        public virtual Color OutColor()
        {
            return DefaultOutColor;
        }

        public virtual Color InColor()
        {
            return DefaultInColor;
        }

        public virtual Color CenterColor()
        {
            return DefaultCenterColor;
        }

        public void CreateColors()
        {
            Color startColor = CenterColor();
            GraphUtil.CreateGradientColorsList(polygonSize, startColor, InColor(), inBorder);
            GraphUtil.CreateGradientColorsList(polygonSize, startColor, OutColor(), outBorder);
        }

        protected override void DrawPrimitives()
        {
            if (OpacityFloat > 0f)
            {
                GraphUtil.FillTrianglesList(inBorder);
                GraphUtil.FillTrianglesList(outBorder);
            }
        }

        int IOpacity.OpacityByte
        {
            get => OpacityByte;
            set => OpacityByte = value;
        }
    }
}
