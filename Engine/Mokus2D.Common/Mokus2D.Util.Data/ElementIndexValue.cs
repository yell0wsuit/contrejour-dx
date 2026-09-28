namespace Mokus2D.Util.Data;

public struct ElementIndexValue<TElement, TValue>(TElement element, int index, TValue value)
{
	public readonly TElement Element = element;

	public readonly int Index = index;

	public readonly TValue Value = value;
}
