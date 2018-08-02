using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLib.Common
{
    public static class LinqCommon
    {
        public static IEnumerable<TSource> Replace<TSource>(this IEnumerable<TSource> source, TSource newValue, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            var e = source.GetEnumerator();
            while (e.MoveNext())
                if (predicate(e.Current))
                    yield return newValue;
                else
                    yield return e.Current;
            e.Dispose();
        }
    }
}
