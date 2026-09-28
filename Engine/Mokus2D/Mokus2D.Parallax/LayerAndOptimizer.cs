namespace Mokus2D.Parallax;

internal sealed class LayerAndOptimizer
{
    public readonly ParallaxLayer Layer;

    public readonly LayerVisibilityOptimizer Optimizer;

    public LayerAndOptimizer(ParallaxLayer layer, LayerVisibilityOptimizer optimizer)
    {
        Layer = layer;
        Optimizer = optimizer;
    }
}
