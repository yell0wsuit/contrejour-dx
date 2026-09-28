using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PlasticineHighliteBorder : PrimitivesNode
{
    public const int HighlitePartVerticesCount = 4;

    private VertexPositionColor[] vertices;

    private List<object> parts = [];

    private PlasticineWideBorder border;

    public VertexPositionColor[] Vertices => vertices;

    public VertexPositionColor[] InBorder => border.InBorder;

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
        vertices = new VertexPositionColor[this.border.OutBorder.Length];
    }

    public VertexPositionColor[] OutBorder()
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
        GraphUtil.DrawTriangleStrip(vertices);
    }
}
