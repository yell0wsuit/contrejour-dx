using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class ShadowNode : SpriteBatchNode
{
    protected List<Vector2> borderPoints;

    protected List<Color> borderColors;

    protected List<Vector2> fillPoints;

    private readonly Color IN_COLOR = new Color(0, 0, 0, 50);

    private readonly Color OUT_COLOR = new Color(0, 0, 0, 0);

    private ShadowNode()
    {
    }

    public void AddShadowInPoints(List<Vector2> outPoints, List<Vector2> inPoints)
    {
        borderPoints.Add(outPoints[0]);
        borderPoints.Add(outPoints[1]);
        borderPoints.Add(inPoints[0]);
        borderPoints.Add(outPoints[1]);
        borderPoints.Add(inPoints[0]);
        borderPoints.Add(inPoints[1]);
        AddBorderColors();
        borderPoints.Add(outPoints[2]);
        borderPoints.Add(outPoints[3]);
        borderPoints.Add(inPoints[2]);
        borderPoints.Add(outPoints[3]);
        borderPoints.Add(inPoints[2]);
        borderPoints.Add(inPoints[3]);
        AddBorderColors();
        fillPoints.Add(inPoints[0]);
        fillPoints.Add(inPoints[1]);
        fillPoints.Add(inPoints[2]);
        fillPoints.Add(inPoints[2]);
        fillPoints.Add(inPoints[0]);
        fillPoints.Add(inPoints[3]);
    }

    public void AddBorderColors()
    {
        borderColors.Add(OUT_COLOR);
        borderColors.Add(OUT_COLOR);
        borderColors.Add(IN_COLOR);
        borderColors.Add(OUT_COLOR);
        borderColors.Add(IN_COLOR);
        borderColors.Add(IN_COLOR);
    }

    public void Clear()
    {
        borderPoints.Clear();
        borderColors.Clear();
        fillPoints.Clear();
    }

    protected override void DrawSprite(VisualState state, Color color)
    {
    }
}
