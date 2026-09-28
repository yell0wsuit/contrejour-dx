using Mokus2D.Data;
using Mokus2D.Visual.Text;

namespace Mokus2D.UI.Grids.ItemRenderers;

public class LabelItemRenderer<T>(string fontName, float fontSize) : Label(fontName, fontSize), IItemRenderer<T>, ICleanable
{
    public void SetData(object sharedData, T itemData, int index)
    {
        TextString = GetText(itemData);
    }

    protected virtual string GetText(T itemData)
    {
        return itemData.ToString();
    }

    public virtual void RefreshPosition(float itemPosition, int itemsCount)
    {
    }

    public void Clean()
    {
    }
}
