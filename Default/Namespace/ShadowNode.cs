using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Visual;
using Mokus2D.Visual.Data;

namespace Default.Namespace;

public class ShadowNode : SpriteBatchNode
{
    private List<Vector2> borderPoints;

    private List<Color> borderColors;

    private List<Vector2> fillPoints;

    private readonly Color InColor = new(0, 0, 0, 50);

    private readonly Color OutColor = new(0, 0, 0, 0);

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
        borderColors.Add(OutColor);
        borderColors.Add(OutColor);
        borderColors.Add(InColor);
        borderColors.Add(OutColor);
        borderColors.Add(InColor);
        borderColors.Add(InColor);
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
