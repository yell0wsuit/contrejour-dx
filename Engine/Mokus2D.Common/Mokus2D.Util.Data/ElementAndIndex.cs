namespace Mokus2D.Util.Data;

public struct ElementAndIndex<T>(T element, int index)
{
    public readonly T Element = element;

    public readonly int Index = index;
}
