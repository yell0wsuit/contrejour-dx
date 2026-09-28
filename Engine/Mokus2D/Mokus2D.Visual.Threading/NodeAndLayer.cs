namespace Mokus2D.Visual.Threading;

public readonly struct NodeAndLayer(Node node, int layer, bool removeFromPreviousParent)
{
    public readonly int Layer = layer;

    public readonly bool RemoveFromPreviousParent = removeFromPreviousParent;

    public readonly Node Node = node;
}
