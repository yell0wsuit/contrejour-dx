using System;

using Microsoft.Xna.Framework;

namespace Default.Namespace;

public class BlackTrampolineSprite(ContreJourGame _game, SnotBodyClipBase _snot, float _startWidth, float _centerWidth, float _endWidth) : BlackSnotSprite(_game, _snot, _startWidth, _centerWidth, _endWidth)
{
    private Color MIDDLE_COLOR = new Color(0, 254, 254, 255);

    private Color START_TRAMPOLINE_COLOR = new Color(0, 94, 118, 255);

    public virtual Color MiddleColor()
    {
        return MIDDLE_COLOR;
    }

    public virtual Color StartColor()
    {
        return START_TRAMPOLINE_COLOR;
    }

    public override Color GetIntermidiateColorLineSize(int index, int lineSize)
    {
        return Color.Lerp(MiddleColor(), StartColor(), Math.Abs((float)index - (float)lineSize / 2f) / ((float)lineSize / 2f));
    }

    public override void CreateVectors(int _allPointsSize)
    {
        base.CreateVectors(_allPointsSize);
    }

    public override void SetBorderColors()
    {
        if (border != null && drawBorder)
        {
            BlackDrawUtil.SetBorderColors(allPointsSize / 2, StartColor(), MiddleColor(), EndColor(), border);
            int num = border.Length / 2;
            for (int i = 0; i < num; i++)
            {
                border[num + i].Color = border[i].Color;
            }
        }
    }
}
