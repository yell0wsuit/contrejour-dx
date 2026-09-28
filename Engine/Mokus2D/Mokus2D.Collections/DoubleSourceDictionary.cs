using System;
using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Collections;

public class DoubleSourceDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
{
	public bool Test;

	private IDictionary<TKey, TValue> _mainSource;

	public IDictionary<TKey, TValue> SecondSource { get; private set; }

	public int Count
	{
		get
		{
			int num = _mainSource.Count;
			if (SecondSource != null)
			{
				num += SecondSource.Count;
			}
			return num;
		}
	}

	public bool IsReadOnly => true;

	public TValue this[TKey key]
	{
		get
		{
			if (SecondSource != null && SecondSource.TryGetValue(key, out var value))
			{
				return value;
			}
			return _mainSource[key];
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public ICollection<TKey> Keys
	{
		get
		{
			if (SecondSource == null)
			{
				return _mainSource.Keys;
			}
			throw new NotImplementedException();
		}
	}

	public ICollection<TValue> Values
	{
		get
		{
			if (SecondSource == null)
			{
				return _mainSource.Values;
			}
			throw new NotImplementedException();
		}
	}

	public DoubleSourceDictionary(IDictionary<TKey, TValue> mainSource)
	{
		_mainSource = mainSource;
	}

	public void SetSecondSource(IDictionary<TKey, TValue> secondSource)
	{
		SecondSource = secondSource;
	}

	public void ResetMainSource(IDictionary<TKey, TValue> mainSource)
	{
		_mainSource = mainSource;
	}

	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		if (SecondSource == null)
		{
			return _mainSource.GetEnumerator();
		}
		throw new NotImplementedException();
	}

	public void Add(KeyValuePair<TKey, TValue> item)
	{
		throw new NotImplementedException();
	}

	public void Clear()
	{
		throw new NotImplementedException();
	}

	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		if (!_mainSource.Contains(item))
		{
			if (SecondSource != null)
			{
				return SecondSource.Contains(item);
			}
			return false;
		}
		return true;
	}

	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		throw new NotImplementedException();
	}

	public bool ContainsKey(TKey key)
	{
		if (!_mainSource.ContainsKey(key))
		{
			if (SecondSource != null)
			{
				return SecondSource.ContainsKey(key);
			}
			return false;
		}
		return true;
	}

	public void Add(TKey key, TValue value)
	{
		throw new NotImplementedException();
	}

	public bool Remove(TKey key)
	{
		throw new NotImplementedException();
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		if (SecondSource != null && SecondSource.TryGetValue(key, out value))
		{
			return true;
		}
		return _mainSource.TryGetValue(key, out value);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		if (SecondSource == null)
		{
			return _mainSource.GetEnumerator();
		}
		throw new NotImplementedException();
	}
}
