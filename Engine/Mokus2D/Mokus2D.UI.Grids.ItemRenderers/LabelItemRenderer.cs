using Mokus2D.Data;
using Mokus2D.Visual.Text;

namespace Mokus2D.UI.Grids.ItemRenderers;

public class LabelItemRenderer<T> : Label, IItemRenderer<T>, ICleanable
{
	public LabelItemRenderer(string fontName, float fontSize)
		: base(fontName, fontSize)
	{
	}

	public void SetData(object sharedData, T itemData, int index)
	{
		base.TextString = GetText(itemData);
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
