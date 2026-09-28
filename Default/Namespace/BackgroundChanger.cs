using System.Collections.Generic;
using Mokus2D.Visual;

namespace Default.Namespace;

public class BackgroundChanger
{
    private readonly List<Sprite> backgrounds;

    private float currentIndex;

    private int firstIndex;

    private int nextIndex;

    private float offset;

    public int FirstIndex => firstIndex;

    public int NextIndex => nextIndex;

    public float Offset => offset;

    public float CurrentIndex
    {
        get
        {
            return currentIndex;
        }
        set
        {
            if (currentIndex != value)
            {
                currentIndex = value;
                RefreshOpacity();
            }
        }
    }

    public BackgroundChanger(List<Sprite> backgrounds)
    {
        this.backgrounds = backgrounds;
        RefreshOpacity();
    }

    private void RefreshOpacity()
    {
        firstIndex = (int)Maths.ModPositive(currentIndex, ContreJourConstants.PlanetsCount);
        offset = Maths.PeriodicOffset(currentIndex - (float)firstIndex, ContreJourConstants.PlanetsCount);
        nextIndex = (firstIndex + 1) % ContreJourConstants.PlanetsCount;
        for (int i = 0; i < backgrounds.Count; i++)
        {
            backgrounds[i].Visible = false;
        }
        SetBackgroundOpacity(firstIndex, offset);
        SetBackgroundOpacity(nextIndex, 1f - offset);
    }

    private void SetBackgroundOpacity(int index, float offset)
    {
        Sprite sprite = backgrounds[index];
        sprite.OpacityFloat = 1f - offset;
        sprite.Visible = sprite.OpacityByte > 0;
    }
}
