using System.Collections;
using System.Collections.Generic;

namespace Mokus2D.Util.Data
{
    public class ReverseCollection<T>(IList<T> source) : IEnumerable<T>, IEnumerable
    {
        private readonly IList<T> source = source;

        public IEnumerator<T> GetEnumerator()
        {
            return new ReverseEnumerator<T>(source);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
