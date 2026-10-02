// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
// Ported from MonoGame 3.8.5.1 for Farseer's AbstractForceController.

using System;
using System.Collections;
using System.Collections.Generic;

namespace FarseerPhysics.Common
{
    /// <summary>
    /// The collection of the <see cref="CurveKey"/> elements and a part of the <see cref="Curve"/> class.
    /// </summary>
    public class CurveKeyCollection : ICollection<CurveKey>
    {
        #region Private Fields

        private readonly List<CurveKey> _keys;

        #endregion

        #region Properties

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">The index of key in this collection.</param>
        /// <returns><see cref="CurveKey"/> at <paramref name="index"/> position.</returns>
        public CurveKey this[int index]
        {
            get => _keys[index];
            set
            {
                ArgumentNullException.ThrowIfNull(value);

                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _keys.Count);

                if (_keys[index].Position == value.Position)
                {
                    _keys[index] = value;
                }
                else
                {
                    _keys.RemoveAt(index);
                    _keys.Add(value);
                }
            }
        }

        /// <summary>
        /// Returns the count of keys in this collection.
        /// </summary>
        public int Count => _keys.Count;

        /// <summary>
        /// Returns false because it is not a read-only collection.
        /// </summary>
        public bool IsReadOnly => false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="CurveKeyCollection"/> class.
        /// </summary>
        public CurveKeyCollection()
        {
            _keys = [];
        }

        #endregion

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _keys.GetEnumerator();
        }


        /// <summary>
        /// Adds a key to this collection.
        /// </summary>
        /// <param name="item">New key for the collection.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="item"/> is null.</exception>
        /// <remarks>The new key would be added respectively to a position of that key and the position of other keys.</remarks>
        public void Add(CurveKey item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (_keys.Count == 0)
            {
                _keys.Add(item);
                return;
            }

            for (int i = 0; i < _keys.Count; i++)
            {
                if (item.Position < _keys[i].Position)
                {
                    _keys.Insert(i, item);
                    return;
                }
            }

            _keys.Add(item);
        }

        /// <summary>
        /// Removes all keys from this collection.
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
        }

        /// <summary>
        /// Creates a copy of this collection.
        /// </summary>
        /// <returns>A copy of this collection.</returns>
        public CurveKeyCollection Clone()
        {
            CurveKeyCollection ckc = [.. _keys];
            return ckc;
        }

        /// <summary>
        /// Determines whether this collection contains a specific key.
        /// </summary>
        /// <param name="item">The key to locate in this collection.</param>
        /// <returns><c>true</c> if the key is found; <c>false</c> otherwise.</returns>
        public bool Contains(CurveKey item)
        {
            return _keys.Contains(item);
        }

        /// <summary>
        /// Copies the keys of this collection to an array, starting at the array index provided.
        /// </summary>
        /// <param name="array">Destination array where elements will be copied.</param>
        /// <param name="arrayIndex">The zero-based index in the array to start copying from.</param>
        public void CopyTo(CurveKey[] array, int arrayIndex)
        {
            _keys.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the <see cref="CurveKeyCollection"/>.</returns>
        public IEnumerator<CurveKey> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        /// <summary>
        /// Finds element in the collection and returns its index.
        /// </summary>
        /// <param name="item">Element for the search.</param>
        /// <returns>Index of the element; or -1 if item is not found.</returns>
        public int IndexOf(CurveKey item)
        {
            return _keys.IndexOf(item);
        }

        /// <summary>
        /// Searches for the key with the lowest position greater than or equal to the specified position.
        /// </summary>
        /// <param name="position">Position to search for.</param>
        /// <returns>The zero-based index of the first matching position if there is a match; otherwise, a negative number that is the bitwise complement of the index of the next element that is larger than <c>position</c> or, if there is no larger element, the bitwise complement of <c>Count</c>.</returns>
        public int IndexAtPosition(float position)
        {
            int index = _keys.BinarySearch(new CurveKey(position, 0));

            if (index < 0)
            {
                return index;
            }

            //If several matching keys exist, return the first one
            while (index - 1 >= 0 && _keys[index - 1].Position == position)
            {
                index--;
            }

            return index;
        }

        /// <summary>
        /// Removes element at the specified index.
        /// </summary>
        /// <param name="index">The index which element will be removed.</param>
        public void RemoveAt(int index)
        {
            _keys.RemoveAt(index);
        }

        /// <summary>
        /// Removes specific element.
        /// </summary>
        /// <param name="item">The element</param>
        /// <returns><c>true</c> if item is successfully removed; <c>false</c> otherwise. This method also returns <c>false</c> if item was not found.</returns>
        public bool Remove(CurveKey item)
        {
            return _keys.Remove(item);
        }
    }
}
