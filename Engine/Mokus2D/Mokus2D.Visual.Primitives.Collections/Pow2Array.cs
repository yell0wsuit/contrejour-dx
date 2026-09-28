using System;
using Default.Namespace;

namespace Mokus2D.Visual.Primitives.Collections;

public class Pow2Array<T>
{
	private const int DefaultCapacity = 128;

	private T[] _items;

	public int Length { get; private set; }

	public T[] Items => _items;

	public Pow2Array()
		: this(128)
	{
	}

	public Pow2Array(int capacity)
	{
		_items = new T[Maths.Pow2Ceil(capacity)];
	}

	public void SetLength(int value)
	{
		EnsureCapacity(value);
		Length = value;
	}

	public void Add(T item)
	{
		EnsureCapacity(Length + 1);
		_items[Length] = item;
		Length++;
	}

	public void EnsureCapacity(int capacity)
	{
		DoEnsureCapacity(Maths.Pow2Ceil(capacity));
	}

	private void DoEnsureCapacity(int capacity)
	{
		if (_items.Length < capacity)
		{
			Array.Resize(ref _items, capacity);
		}
	}

	public void Clear()
	{
		Length = 0;
	}
}
