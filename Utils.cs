using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    internal static class Utils
    {
        static readonly Random random = new Random(DateTime.Now.Millisecond);
        internal static T Random<T>(this IEnumerable<T> items)
        {
            if (items == null || items.Count() == 0) return default(T);
            else return items.Skip(random.Next(items.Count())).First();
        }

        internal static IEnumerable<string> Dupp(this string str, int times)
        {
            for(int i = 0; i< times; i++)
            {
                yield return str;
            }
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] values)
            => source.Except(values.AsEnumerable());

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, params T[] values)
            => source.Concat(values.AsEnumerable());
			
        public static IEnumerable<T> ConcatIf<T>(this IEnumerable<T> source, bool val, params T[] values)
            => val ? source.Concat(values.AsEnumerable()) : source;
    }
}
