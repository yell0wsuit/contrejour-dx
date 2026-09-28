using System;
using System.Collections;
using System.Collections.Generic;

using Mokus2D.Collections.ForEach;

namespace Mokus2D.Collections;

public class ForEachDebugList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IForEachList
{
    private readonly List<T> _list = new List<T>();

    private bool _inForEach;

    public object SyncRoot => ((ICollection)_list).SyncRoot;

    public bool IsSynchronized => ((ICollection)_list).IsSynchronized;

    T IList<T>.this[int index]
    {
        get
        {
            return _list[index];
        }
        set
        {
            ThrowIfInForEach();
            _list[index] = value;
        }
    }

    public int Count => _list.Count;

    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    public T this[int index]
    {
        get
        {
            return _list[index];
        }
        set
        {
            ThrowIfInForEach();
            _list[index] = value;
        }
    }

    public int Capacity
    {
        get
        {
            return _list.Capacity;
        }
        set
        {
            _list.Capacity = value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }

    public int Add(object value)
    {
        ThrowIfInForEach();
        return ((IList)_list).Add(value);
    }

    public bool Contains(object value)
    {
        return ((IList)_list).Contains(value);
    }

    public int IndexOf(object value)
    {
        return ((IList)_list).IndexOf(value);
    }

    public void Insert(int index, object value)
    {
        ThrowIfInForEach();
        ((IList)_list).Insert(index, value);
    }

    public void Remove(object value)
    {
        ThrowIfInForEach();
        ((IList)_list).Remove(value);
    }

    public void Add(T item)
    {
        ThrowIfInForEach();
        _list.Add(item);
    }

    public void AddRange(IEnumerable<T> collection)
    {
        ThrowIfInForEach();
        _list.AddRange(collection);
    }

    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        return _list.BinarySearch(index, count, item, comparer);
    }

    public int BinarySearch(T item)
    {
        return _list.BinarySearch(item);
    }

    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return _list.BinarySearch(item, comparer);
    }

    public void Clear()
    {
        ThrowIfInForEach();
        _list.Clear();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array)
    {
        _list.CopyTo(array);
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        _list.CopyTo(index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Exists(Predicate<T> match)
    {
        return _list.Exists(match);
    }

    public T Find(Predicate<T> match)
    {
        return _list.Find(match);
    }

    public List<T> FindAll(Predicate<T> match)
    {
        return _list.FindAll(match);
    }

    public int FindIndex(Predicate<T> match)
    {
        return _list.FindIndex(match);
    }

    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, match);
    }

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindIndex(startIndex, count, match);
    }

    public T FindLast(Predicate<T> match)
    {
        return _list.FindLast(match);
    }

    public int FindLastIndex(Predicate<T> match)
    {
        return _list.FindLastIndex(match);
    }

    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, match);
    }

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return _list.FindLastIndex(startIndex, count, match);
    }

    public List<T>.Enumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public List<T> GetRange(int index, int count)
    {
        return _list.GetRange(index, count);
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public int IndexOf(T item, int index)
    {
        return _list.IndexOf(item, index);
    }

    public int IndexOf(T item, int index, int count)
    {
        return _list.IndexOf(item, index, count);
    }

    public void Insert(int index, T item)
    {
        ThrowIfInForEach();
        _list.Insert(index, item);
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        ThrowIfInForEach();
        _list.InsertRange(index, collection);
    }

    public int LastIndexOf(T item)
    {
        return _list.LastIndexOf(item);
    }

    public int LastIndexOf(T item, int index)
    {
        return _list.LastIndexOf(item, index);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        return _list.LastIndexOf(item, index, count);
    }

    public bool Remove(T item)
    {
        ThrowIfInForEach();
        return _list.Remove(item);
    }

    public int RemoveAll(Predicate<T> match)
    {
        ThrowIfInForEach();
        return _list.RemoveAll(match);
    }

    public void RemoveAt(int index)
    {
        ThrowIfInForEach();
        _list.RemoveAt(index);
    }

    public void RemoveRange(int index, int count)
    {
        ThrowIfInForEach();
        _list.RemoveRange(index, count);
    }

    public void Reverse()
    {
        ThrowIfInForEach();
        _list.Reverse();
    }

    public void Reverse(int index, int count)
    {
        ThrowIfInForEach();
        _list.Reverse(index, count);
    }

    public void Sort()
    {
        ThrowIfInForEach();
        _list.Sort();
    }

    public void Sort(IComparer<T> comparer)
    {
        ThrowIfInForEach();
        _list.Sort(comparer);
    }

    public void Sort(int index, int count, IComparer<T> comparer)
    {
        ThrowIfInForEach();
        _list.Sort(index, count, comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        ThrowIfInForEach();
        _list.Sort(comparison);
    }

    public T[] ToArray()
    {
        return _list.ToArray();
    }

    public void TrimExcess()
    {
        _list.TrimExcess();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        return _list.TrueForAll(match);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void StartForEach()
    {
        _inForEach = true;
    }

    public void EndForEach()
    {
        _inForEach = false;
    }

    public ForEachListUsing Using()
    {
        return new ForEachListUsing(this);
    }

    private void ThrowIfInForEach()
    {
        if (_inForEach)
        {
            throw new Exception("List modified in for each");
        }
    }
}
