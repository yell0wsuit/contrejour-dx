using System.Collections.Generic;

using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public class BackgroundChanger
    {
        private readonly List<Sprite> backgrounds;

        public int FirstIndex { get; private set; }

        public int NextIndex { get; private set; }

        public float Offset { get; private set; }

        public float CurrentIndex
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
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
            FirstIndex = (int)Maths.ModPositive(CurrentIndex, ContreJourConstants.PlanetsCount);
            Offset = Maths.PeriodicOffset(CurrentIndex - FirstIndex, ContreJourConstants.PlanetsCount);
            NextIndex = (FirstIndex + 1) % ContreJourConstants.PlanetsCount;
            for (int i = 0; i < backgrounds.Count; i++)
            {
                backgrounds[i].Visible = false;
            }
            SetBackgroundOpacity(FirstIndex, Offset);
            SetBackgroundOpacity(NextIndex, 1f - Offset);
        }

        private void SetBackgroundOpacity(int index, float offset)
        {
            Sprite sprite = backgrounds[index];
            sprite.OpacityFloat = 1f - offset;
            sprite.Visible = sprite.OpacityByte > 0;
        }
    }
}
