using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.UI.Grids.ItemRenderers
{
    public static class ItemRenderersUtil
    {
        public static void AdjustOffsetItemsOpacity(Node itemRenderer, float itemPosition, int itemsCount, float maxOffset)
        {
            float num = 0f;
            if (itemPosition < 0f)
            {
                num = 0f - itemPosition;
            }
            else if (itemPosition > itemsCount - 1)
            {
                num = itemPosition - (itemsCount - 1);
            }
            itemRenderer.OpacityFloat = (num / maxOffset).Lerp(1f, 0f);
        }
    }
}
