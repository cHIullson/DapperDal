using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperExtensions.Expressions
{
    /// <summary>
    /// 排序方向
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// 升序
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// 降序
        /// </summary>
        Descending
    }

    public static class SortingExtensions
    {
        public static IList<ISort> ToSortable<T>(this Expression<Func<T, object>>[] sortingExpression,
            SortDirection ascending = SortDirection.Ascending)
        {
            if (sortingExpression == null)
            {
                return null;
            }

            var sortList = new List<ISort>();
            sortingExpression.ToList().ForEach(sortExpression =>
            {
                MemberInfo sortProperty = ReflectionHelper.GetProperty(sortExpression);
                sortList.Add(new Sort
                {
                    Ascending = ascending != SortDirection.Descending,
                    PropertyName = sortProperty.Name
                });
            });

            return sortList;
        }

        public static IList<ISort> ToSortable(this object sort)
        {
            if (sort == null)
            {
                return null;
            }

            var sortList = sort as IList<Sort>;
            if (sortList != null)
            {
                return new List<ISort>(sortList);
            }

            var sorts = new List<ISort>();
            foreach (var kvp in ReflectionHelper.GetObjectValues(sort))
            {
                var ascending = true;

                if (false.Equals(kvp.Value))
                {
                    ascending = false;
                }

                if (SortDirection.Descending.Equals(kvp.Value))
                {
                    ascending = false;
                }

                sorts.Add(new Sort { PropertyName = kvp.Key, Ascending = ascending });
            }

            return sorts;
        }

    }
}
