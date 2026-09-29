namespace Mokus2D.Parallax
{
    internal sealed class LayerAndOptimizer(ParallaxLayer layer, LayerVisibilityOptimizer optimizer)
    {
        public readonly ParallaxLayer Layer = layer;

        public readonly LayerVisibilityOptimizer Optimizer = optimizer;
    }
}
