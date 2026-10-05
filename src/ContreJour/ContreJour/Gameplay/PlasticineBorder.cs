using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Graphics;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay
{
    public class PlasticineBorder : PrimitivesNode, IOpacity
    {
        private readonly Vertex[] outBorder;

        private readonly Vertex[] inBorder;

        private readonly int polygonSize;

        // iOS fades outward to transparent white; primitives interpolate straight alpha.
        private static readonly Color DefaultOutColor = new(255, 255, 255, 0);

        private static readonly Color DefaultInColor = new(0, 0, 0, 0);

        private static readonly Color DefaultCenterColor = new(127, 127, 127);

        private readonly float WIDTH = 2f;

        // iOS scales only the center vertex alpha. Node opacity would also
        // scale the primitive's color and darken the outline.
        public int Opacity
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
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
            // iOS turns +90 degrees from the outline for the in side; GetOutVertex turns -90.
            GraphUtil.CreateGradientBorderWidthVertices(surface, 0f - BorderWidth(), inBorder);
            GraphUtil.CreateGradientBorderWidthVertices(surface, BorderWidth(), outBorder);
            CreateColors();
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

        protected Color ScaledCenterColor()
        {
            Color center = CenterColor();
            return center.ChangeAlpha((byte)(Opacity / 255f * center.A));
        }

        public void CreateColors()
        {
            Color startColor = ScaledCenterColor();
            GraphUtil.CreateGradientColorsList(polygonSize, startColor, InColor(), inBorder);
            GraphUtil.CreateGradientColorsList(polygonSize, startColor, OutColor(), outBorder);
        }

        protected override void DrawPrimitives()
        {
            if (Opacity >= 1)
            {
                GraphUtil.FillTrianglesList(inBorder);
                GraphUtil.FillTrianglesList(outBorder);
            }
        }

        int IOpacity.OpacityByte
        {
            get => Opacity;
            set => Opacity = value;
        }
    }
}
