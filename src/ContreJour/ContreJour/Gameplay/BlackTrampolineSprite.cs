using System;

using Microsoft.Xna.Framework;

namespace ContreJour.Gameplay;

public class BlackTrampolineSprite(ContreJourGame game, SnotBodyClipBase snot, float startWidth, float centerWidth, float endWidth) : BlackSnotSprite(game, snot, startWidth, centerWidth, endWidth)
{
    private Color DefaultMiddleColor = new(0, 254, 254, 255);

    private Color StartTrampolineColor = new(0, 94, 118, 255);

    public virtual Color MiddleColor()
    {
        return DefaultMiddleColor;
    }

    public virtual Color StartColor()
    {
        return StartTrampolineColor;
    }

    public override Color GetIntermidiateColorLineSize(int index, int lineSize)
    {
        return Color.Lerp(MiddleColor(), StartColor(), Math.Abs(index - (lineSize / 2f)) / (lineSize / 2f));
    }

    public override void CreateVectors(int allPointsSize)
    {
        base.CreateVectors(allPointsSize);
    }

    public override void SetBorderColors()
    {
        if (Border != null)
        {
            BlackDrawUtil.SetBorderColors(AllPointsSize / 2, StartColor(), MiddleColor(), EndColor(), Border);
            int num = Border.Length / 2;
            for (int i = 0; i < num; i++)
            {
                Border[num + i].Color = Border[i].Color;
            }
        }
    }
}
