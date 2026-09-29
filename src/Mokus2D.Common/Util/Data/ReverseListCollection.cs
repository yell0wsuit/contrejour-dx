using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data
{
    public readonly struct ReverseListCollection<T>(IList<T> list) : IEnumerable<T>, IEnumerable
    {
        private readonly IList<T> _list = list;

        public readonly IEnumerator<T> GetEnumerator()
        {
            return new ReverseListEnumerator<T>(_list);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
