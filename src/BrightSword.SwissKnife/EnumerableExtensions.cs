using System;
using System.Collections.Generic;
using System.Linq;

namespace BrightSword.SwissKnife
{
    public static class EnumerableExtensions
    {
        public static bool AllUnique<T>(this IEnumerable<T> _this)
        {
            return _this.All(new HashSet<T>().Add);
        }

        public static bool SortedListIsUnique<T>(this IList<T> _this)
        {
            for (int _prev = -1,
                     _curr = 0;
                 _curr < _this.Count;
                 _prev = _curr++)
            {
                if (_prev == -1) { continue; }
                if (_this[_prev].Equals(_this[_curr])) { return false; }
            }
            return true;
        }

        public static T LastButOne<T>(this IEnumerable<T> _this)
        {
            if (_this == null) { return default(T); }

            return _this.Reverse()
                        .Skip(1)
                        .FirstOrDefault();
        }

        public static bool SequenceEqual<T>(this IEnumerable<T> _this, IList<T> other) where T : IEquatable<T>
        {
            return _this.Select((_item, _index) => _item.Equals(other[_index]))
                        .All(_ => _);
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> _this, int batchSize = 100)
        {
            var currentBatch = new List<T>();

            foreach (var item in _this)
            {
                if (currentBatch.Count < batchSize) {
                    currentBatch.Add(item);
                }
                else
                {
                    yield return currentBatch;

                    currentBatch = new List<T>
                                   {
                                       item
                                   };
                }
            }

            yield return currentBatch;
        }
    }
}