using System;
using System.Collections.Generic;

namespace Mokus2D.Collections;

public class FactoryDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    private readonly Func<TKey, TValue> _factory;

    public FactoryDictionary(Func<TKey, TValue> factory)
    {
        _factory = factory;
    }

    public TValue GetOrCreate(TKey key)
    {
        if (!TryGetValue(key, out TValue value))
        {
            value = _factory(key);
            Add(key, value);
        }
        return value;
    }
}
