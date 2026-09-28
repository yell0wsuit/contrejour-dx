namespace Mokus2D.Util.Data;

public readonly struct ElementIndexValue<TElement, TValue>(TElement element, int index, TValue value)
{
    private readonly TElement Element = element;

    private readonly int Index = index;

    private readonly TValue Value = value;
}
