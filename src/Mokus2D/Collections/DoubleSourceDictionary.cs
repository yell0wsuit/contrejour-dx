using System;
using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Collections
{
    public class DoubleSourceDictionary<TKey, TValue>(IDictionary<TKey, TValue> mainSource) : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
    {
        private IDictionary<TKey, TValue> _mainSource = mainSource;

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
            get => SecondSource != null && SecondSource.TryGetValue(key, out TValue value) ? value : _mainSource[key];

            set => throw new NotImplementedException();
        }

        public ICollection<TKey> Keys => SecondSource == null ? _mainSource.Keys : throw new NotImplementedException();

        public ICollection<TValue> Values => SecondSource == null ? _mainSource.Values : throw new NotImplementedException();

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
            return SecondSource == null ? _mainSource.GetEnumerator() : throw new NotImplementedException();
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
            return _mainSource.Contains(item) || (SecondSource != null && SecondSource.Contains(item));
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
            return _mainSource.ContainsKey(key) || (SecondSource != null && SecondSource.ContainsKey(key));
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
            return (SecondSource != null && SecondSource.TryGetValue(key, out value)) || _mainSource.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SecondSource == null ? (IEnumerator)_mainSource.GetEnumerator() : throw new NotImplementedException();
        }
    }
}
