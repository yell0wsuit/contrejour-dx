using System.Collections;
using System.Collections.Generic;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Collections;

public class BiDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
    private readonly IDictionary<TKey, TValue> _firstToSecond = new Dictionary<TKey, TValue>();

    private readonly Dictionary<TValue, TKey> _secondToFirst = [];

    public int Count => _firstToSecond.Count;

    public bool IsReadOnly => _firstToSecond.IsReadOnly;

    public TValue this[TKey key]
    {
        get => _firstToSecond[key];
        set
        {
            _firstToSecond[key] = value;
            _secondToFirst[value] = key;
        }
    }

    public TKey this[TValue value] => _secondToFirst[value];

    public ICollection<TKey> Keys => _firstToSecond.Keys;

    public ICollection<TValue> Values => _firstToSecond.Values;

    public void Add(TKey first, TValue second)
    {
        _firstToSecond.Add(first, second);
        _secondToFirst.Add(second, first);
    }

    public TKey GetKey(TValue value)
    {
        return _secondToFirst[value];
    }

    public TKey TryGetKey(TValue value)
    {
        return _secondToFirst.TryGetValue(value);
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _firstToSecond.GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _firstToSecond.Add(item);
        _secondToFirst.Add(item.Value, item.Key);
    }

    public void Clear()
    {
        _firstToSecond.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _firstToSecond.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _firstToSecond.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _firstToSecond.Remove(item);
    }

    public bool ContainsKey(TKey key)
    {
        return _firstToSecond.ContainsKey(key);
    }

    public bool ContainsValue(TValue value)
    {
        return _secondToFirst.ContainsKey(value);
    }

    public bool Remove(TKey key)
    {
        _ = _secondToFirst.Remove(_firstToSecond[key]);
        return _firstToSecond.Remove(key);
    }

    public bool Remove(TValue value)
    {
        _ = _firstToSecond.Remove(_secondToFirst[value]);
        return _secondToFirst.Remove(value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _firstToSecond.TryGetValue(key, out value);
    }

    public bool TryGetKey(TValue key, out TKey value)
    {
        return _secondToFirst.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
