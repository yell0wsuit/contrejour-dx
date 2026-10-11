using System.Collections.Generic;
using System.Linq;

using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJourDX.Gameplay
{
    public class PlasticineHighliteBorder : PrimitivesNode
    {
        public const int HighlitePartVerticesCount = 4;
        private readonly List<object> parts = [];

        private readonly PlasticineWideBorder border;

        public Vertex[] Vertices { get; }

        public Vertex[] InBorder => border.InBorder;

        public Color MainColor => border.Color;

        public PlasticineHighliteBorder(PlasticineItem firstItem, PlasticineWideBorder border)
        {
            PlasticineItem plasticineItem = firstItem;
            int num = 0;
            do
            {
                PlasticinePartHighlite plasticinePartHighlite = new(plasticineItem.BodyClip, this, (num * 2 * 2) + 2);
                parts.Add(plasticinePartHighlite);
                plasticinePartHighlite.SetDirty();
                plasticineItem = plasticineItem.NextItem;
                num++;
            }
            while (plasticineItem != firstItem);
            this.border = border;
            Vertices = new Vertex[this.border.OutBorder.Length];
        }

        public Vertex[] OutBorder()
        {
            return border.OutBorder;
        }

        public override void Update(float time)
        {
            foreach (PlasticinePartHighlite part in parts.Cast<PlasticinePartHighlite>())
            {
                part.Update(time);
            }
            foreach (PlasticinePartHighlite part2 in parts.Cast<PlasticinePartHighlite>())
            {
                part2.TryRefresh();
            }
        }

        protected override void DrawPrimitives()
        {
            GraphUtil.DrawTriangleStrip(Vertices);
        }
    }
}
