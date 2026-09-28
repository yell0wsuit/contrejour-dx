using System;
using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Visual.Data;

public class SortedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	private Comparison<T> _comparison;

	private IComparer<T> _comparer;

	protected readonly List<T> Items;

	private Comparison<T> Comparison
	{
		set
		{
			_comparison = value;
			_comparer = new ComparisonComparer<T>(_comparison);
		}
	}

	public T this[int index]
	{
		get
		{
			return Items[index];
		}
		set
		{
			throw new NotSupportedException();
		}
	}

	public int Capacity
	{
		get
		{
			return Items.Capacity;
		}
		set
		{
			Items.Capacity = value;
		}
	}

	public int Count => Items.Count;

	public bool IsReadOnly => false;

	public SortedList(IComparer<T> comparer, int capacity)
	{
		_comparer = comparer;
		Items = new List<T>(capacity);
	}

	public SortedList(Comparison<T> comparison)
	{
		Comparison = comparison;
		Items = new List<T>();
	}

	public SortedList(int capacity, Comparison<T> comparison)
	{
		Comparison = comparison;
		Items = new List<T>(capacity);
	}

	public SortedList(IEnumerable<T> collection, Comparison<T> comparison)
	{
		Comparison = comparison;
		Items = new List<T>(collection);
		Items.Sort(comparison);
	}

	public void Add(T item)
	{
		int insertIndex = GetInsertIndex(item);
		Items.Insert(insertIndex, item);
	}

	public virtual int GetInsertIndex(T item, IComparer<T> comparerOverride = null)
	{
		int num = Items.BinarySearch(item, comparerOverride ?? _comparer);
		if (num < 0)
		{
			num = ~num;
		}
		return num;
	}

	public void AddRange(IEnumerable<T> collection)
	{
		foreach (T item in collection)
		{
			Add(item);
		}
	}

	public void Clear()
	{
		Items.Clear();
	}

	public bool Contains(T item)
	{
		return Items.Contains(item);
	}

	public void CopyTo(T[] array)
	{
		Items.CopyTo(array);
	}

	public void CopyTo(int index, T[] array, int arrayIndex, int count)
	{
		Items.CopyTo(index, array, arrayIndex, count);
	}

	public void CopyTo(T[] array, int arrayIndex)
	{
		Items.CopyTo(array, arrayIndex);
	}

	public void ForEach(Action<T> action)
	{
		Items.Each(action);
	}

	public List<T> GetRange(int index, int count)
	{
		return Items.GetRange(index, count);
	}

	public int IndexOf(T item)
	{
		return Items.IndexOf(item);
	}

	public int IndexOf(T item, int index)
	{
		return Items.IndexOf(item, index);
	}

	public int IndexOf(T item, int index, int count)
	{
		return Items.IndexOf(item, index, count);
	}

	public virtual void Insert(int index, T item)
	{
		throw new NotSupportedException();
	}

	public void InsertRange(int index, IEnumerable<T> collection)
	{
		throw new NotSupportedException();
	}

	public int LastIndexOf(T item)
	{
		return Items.LastIndexOf(item);
	}

	public int LastIndexOf(T item, int index)
	{
		return Items.LastIndexOf(item, index);
	}

	public int LastIndexOf(T item, int index, int count)
	{
		return Items.LastIndexOf(item, index, count);
	}

	public virtual bool Remove(T item)
	{
		int num = Items.BinarySearch(item, _comparer);
		if (num >= 0)
		{
			Items.RemoveAt(num);
			return true;
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		Items.RemoveAt(index);
	}

	public void RemoveRange(int index, int count)
	{
		Items.RemoveRange(index, count);
	}

	public void Reverse()
	{
		throw new NotSupportedException();
	}

	public void Reverse(int index, int count)
	{
		throw new NotSupportedException();
	}

	public T[] ToArray()
	{
		return Items.ToArray();
	}

	public List<T>.Enumerator GetEnumerator()
	{
		return Items.GetEnumerator();
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void Sort()
	{
		Items.Sort(_comparer);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
