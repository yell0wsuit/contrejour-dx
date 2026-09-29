using System;

namespace Mokus2D.Visual.Optimization
{
    public class LazyNode<T> : LazyFactoryNode<T> where T : Node, new()
    {
        public LazyNode(Action<T> initialization, Node parent = null, int layer = 0)
            : base(delegate
            {
                T val = new();
                initialization(val);
                return val;
            }, parent, layer)
        {
        }

        public LazyNode(Node parent = null, int layer = 0)
            : base(() => new T(), parent, layer)
        {
        }
    }
}
