using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using DapperExtensions;
using DapperExtensions.Expressions;

namespace DapperDal
{
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用谓词或匿名对象，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(object predicate, object sort,
            int firstResult, int maxResults)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetSet<TEntity>(predicate, sort.ToSortable(),
                    firstResult, maxResults);
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用谓词或匿名对象，排序表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(object predicate,
            int firstResult, int maxResults,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetSet<TEntity>(predicate,
                    sortingExpression.ToSortable(ascending),
                    firstResult, maxResults);
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用表达式，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, object sort,
            int firstResult, int maxResults)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetSet<TEntity>(
                    predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sort.ToSortable(),
                    firstResult, maxResults);
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用表达式，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate,
            int firstResult, int maxResults,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetSet<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sortingExpression.ToSortable(ascending),
                    firstResult, maxResults);
            }
        }
    }
}
