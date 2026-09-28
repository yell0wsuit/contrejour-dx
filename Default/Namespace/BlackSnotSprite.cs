using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class BlackSnotSprite : SpringSnotSprite
{
    private readonly Color END_COLOR = new Color(0, 254, 254, 255);

    private readonly Color START_COLOR = new Color(0, 94, 118, 255);

    public BlackSnotSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth)
        : base(_game, _snot, _startWidth, _centerWidth, _endWidth)
    {
        borderWidth = 3f;
    }

    public int PointsInNeckPart()
    {
        return 4;
    }

    public virtual Color initialStartColor()
    {
        return START_COLOR;
    }

    public virtual Color initialEndColor()
    {
        return END_COLOR;
    }

    public override Color BaseCircleColor()
    {
        return initialStartColor();
    }

    public override Color EndCircleColor()
    {
        return initialEndColor();
    }

    public override Color EndColor()
    {
        Color result = EndCircleColor().Mult(activeProgress);
        result.A = 0;
        return result;
    }

    public virtual Color GetIntermidiateColorLineSize(int index, int lineSize)
    {
        return Color.Lerp(initialStartColor(), initialEndColor(), (float)index / (float)lineSize);
    }

    public override void CreateVectors(int _allPointsSize)
    {
        base.CreateVectors(_allPointsSize);
        int num = _allPointsSize / 2;
        Color color = initialStartColor();
        Color intermidiateColorLineSize = GetIntermidiateColorLineSize(1, num);
        for (int i = 0; i < num - 1; i++)
        {
            int num2 = i * 6;
            vertices[num2].Color = color;
            vertices[num2 + 1].Color = intermidiateColorLineSize;
            vertices[num2 + 2].Color = color;
            vertices[num2 + 3].Color = color;
            vertices[num2 + 4].Color = intermidiateColorLineSize;
            vertices[num2 + 5].Color = intermidiateColorLineSize;
            color = intermidiateColorLineSize;
            intermidiateColorLineSize = GetIntermidiateColorLineSize(i + 2, num);
        }
    }

    protected override void SetNeckColors()
    {
    }

    public override void SetBorderColors()
    {
        BlackDrawUtil.SetBorderColors(allPointsSize, initialStartColor(), initialEndColor(), EndColor(), border);
    }

    public override void DrawPolygons()
    {
        GraphUtil.FillTrianglesList(vertices);
    }
}
