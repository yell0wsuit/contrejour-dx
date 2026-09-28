using System;
using System.Collections.Generic;

namespace Mokus2D.Data;

public class Pool<T>
{
	private Func<T> _activator;

	private readonly List<T> _items = new List<T>(64);

	public int? MaxCount;

	public int ObjectsInPool => _items.Count;

	public Pool()
	{
	}

	public Pool(Func<T> activator)
	{
		SetActivator(activator);
	}

	public void SetActivator(Func<T> activator)
	{
		if (_activator != null)
		{
			throw new Exception("activator is already set");
		}
		_activator = activator;
	}

	public T New()
	{
		if (MaxCount.HasValue && _items.Count >= MaxCount)
		{
			throw new Exception("Pool is full");
		}
		T result;
		if (_items.Count > 0)
		{
			lock (_items)
			{
				result = _items.Last();
				_items.RemoveLast();
			}
		}
		else
		{
			result = _activator();
		}
		return result;
	}

	public void Free(T obj)
	{
		lock (_items)
		{
			if (obj is ICleanable cleanable)
			{
				cleanable.Clean();
			}
			_items.Add(obj);
		}
	}

	public void Clear()
	{
		_items.Clear();
	}
}
