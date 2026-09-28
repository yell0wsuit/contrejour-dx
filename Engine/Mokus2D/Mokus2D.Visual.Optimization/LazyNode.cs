using System;

namespace Mokus2D.Visual.Optimization;

public class LazyNode<T> : LazyFactoryNode<T> where T : Node, new()
{
	public LazyNode(Action<T> initialization, Node parent = null, int layer = 0)
		: base((Func<T>)delegate
		{
			T val = new T();
			initialization(val);
			return val;
		}, parent, layer)
	{
	}

	public LazyNode(Node parent = null, int layer = 0)
		: base((Func<T>)(() => new T()), parent, layer)
	{
	}
}
