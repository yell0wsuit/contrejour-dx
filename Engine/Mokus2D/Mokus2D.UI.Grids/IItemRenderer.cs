using Mokus2D.Data;

namespace Mokus2D.UI.Grids;

public interface IItemRenderer<in T> : ICleanable
{
	void SetData(object sharedData, T itemData, int index);

	void RefreshPosition(float itemPosition, int itemsCount);
}
