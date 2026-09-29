using System;
using System.Collections;
using System.Collections.Generic;

using Mokus2D.Collections.ForEach;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Collections;

public class ForEachCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IForEachList
{
    private readonly List<T> _list;
    private readonly List<T> _toAdd = [];

    private bool _clean;

    private bool _inForEach;

    public List<T> ToRemove { get; } = [];

    public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

    T IList<T>.this[int index]
    {
        get => _list[index];
        set
        {
            ThrowIfForEach();
            _list[index] = value;
        }
    }

    public T this[int index]
    {
        get => _list[index];
        set
        {
            ThrowIfForEach();
            _list[index] = value;
        }
    }

    public int Capacity
    {
        get => _list.Capacity;
        set => _list.Capacity = value;
    }

    public int Count => _list.Count;

    public bool Empty => Count == 0;

    public ForEachCollection(int capacity)
    {
        _list = new List<T>(capacity);
    }

    public ForEachCollection()
    {
        _list = [];
    }

    public void StartForEach()
    {
        _inForEach = true;
    }

    public void EndForEach()
    {
        if (_clean)
        {
            _clean = false;
            _list.Clear();
        }
        if (!ToRemove.Empty())
        {
            _list.RemoveListNoGarbage(ToRemove);
            ToRemove.Clear();
        }
        if (!_toAdd.Empty())
        {
            _list.AddItemsNoGarbage(_toAdd);
            _toAdd.Clear();
        }
        _inForEach = false;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_list).GetEnumerator();
    }

    public List<T>.Enumerator GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)_list).CopyTo(array, index);
    }

    public void Add(T item)
    {
        if (_inForEach)
        {
            _toAdd.Add(item);
        }
        else
        {
            _list.Add(item);
        }
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
        ThrowIfForEach();
        ((IList)_list).Insert(index, value);
    }

    public void Remove(object value)
    {
        if (_inForEach)
        {
            ToRemove.Add((T)value);
        }
        else
        {
            ((IList)_list).Remove(value);
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        ThrowIfForEach();
        _list.AddRange(collection);
    }

    private void ThrowIfForEach()
    {
        if (_inForEach)
        {
            throw new NotImplementedException();
        }
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
        Clear(clearNewAddedActions: false);
    }

    public void Clear(bool clearNewAddedActions)
    {
        if (_inForEach)
        {
            if (clearNewAddedActions)
            {
                _toAdd.Clear();
            }
            ToRemove.Clear();
            _clean = true;
        }
        else
        {
            _list.Clear();
        }
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
        ThrowIfForEach();
        _list.CopyTo(index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ThrowIfForEach();
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
        ThrowIfForEach();
        _list.Insert(index, item);
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        ThrowIfForEach();
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
        if (_inForEach)
        {
            ToRemove.Add(item);
            return true;
        }
        return _list.Remove(item);
    }

    public int RemoveAll(Predicate<T> match)
    {
        ThrowIfForEach();
        return _list.RemoveAll(match);
    }

    public void RemoveAt(int index)
    {
        ThrowIfForEach();
        _list.RemoveAt(index);
    }

    public void RemoveRange(int index, int count)
    {
        ThrowIfForEach();
        _list.RemoveRange(index, count);
    }

    public void Reverse()
    {
        ThrowIfForEach();
        _list.Reverse();
    }

    public void Reverse(int index, int count)
    {
        ThrowIfForEach();
        _list.Reverse(index, count);
    }

    public void Sort()
    {
        ThrowIfForEach();
        _list.Sort();
    }

    public void Sort(IComparer<T> comparer)
    {
        ThrowIfForEach();
        _list.Sort(comparer);
    }

    public void Sort(int index, int count, IComparer<T> comparer)
    {
        ThrowIfForEach();
        _list.Sort(index, count, comparer);
    }

    public void Sort(Comparison<T> comparison)
    {
        ThrowIfForEach();
        _list.Sort(comparison);
    }

    public T[] ToArray()
    {
        ThrowIfForEach();
        return [.. _list];
    }

    public void TrimExcess()
    {
        ThrowIfForEach();
        _list.TrimExcess();
    }

    public bool TrueForAll(Predicate<T> match)
    {
        ThrowIfForEach();
        return _list.TrueForAll(match);
    }

    public ForEachListUsing Using()
    {
        return new ForEachListUsing(this);
    }
}
