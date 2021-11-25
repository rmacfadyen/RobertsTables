using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobertsTables.Code.Tables
{
    public static class SortingExtensions
    {

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, bool ascending, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector)
        {
            if (ascending)
            {
                return source.OrderBy(keySelector);
            }

            return source.OrderByDescending(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, bool ascending, System.Linq.Expressions.Expression<Func<TSource, TKey>> keySelector)
        {
            if (ascending)
            {
                return source.ThenBy(keySelector);
            }

            return source.ThenByDescending(keySelector);
        }

    }
}
