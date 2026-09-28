namespace Mokus2D.Util.Data;

public readonly struct ElementAndIndex<T>(T element, int index)
{
    private readonly T Element = element;

    private readonly int Index = index;
}
