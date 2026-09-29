using System;

namespace Mokus2D.Visual.Optimization
{
    public class LazyFactoryNode<T> where T : Node
    {
        private readonly Lazy<T> _value;

        private readonly Func<T> _factory;

        private readonly Node _parent;

        private readonly int _layer;

        public T Value => _value.Value;

        public LazyFactoryNode(Func<T> factory, Node parent = null, int layer = 0)
        {
            _factory = factory;
            _value = new Lazy<T>(CreateNode);
            _parent = parent;
            _layer = layer;
        }

        private T CreateNode()
        {
            T val = _factory();
            _parent?.AddChild(val, _layer);
            return val;
        }
    }
}
