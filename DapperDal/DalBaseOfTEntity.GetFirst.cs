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
        /// 获取所有实体列表的第一条
        /// </summary>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst()
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件获取实体列表的第一条
        /// （查询使用谓词或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(object predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据排序条件获取所有实体列表的第一条
        /// （排序使用表达式）
        /// </summary>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, null,
                    sortingExpression.ToSortable(ascending)).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表的第一条
        /// （查询使用谓词或匿名对象，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(object predicate, object sort)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate, sort.ToSortable()).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表的第一条
        /// （查询使用谓词或匿名对象，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(object predicate,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate,
                    sortingExpression.ToSortable(ascending)).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件获取实体列表的第一条
        /// （查询使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate.ToPredicateGroup<TEntity, TPrimaryKey>()).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表的第一条
        /// （查询使用表达式，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> predicate, object sort)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sort.ToSortable()).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表的第一条
        /// （查询使用表达式，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> predicate,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (var connection = OpenConnection())
            {
                return connection.GetTop<TEntity>(1, predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sortingExpression.ToSortable(ascending)).FirstOrDefault();
            }
        }
    }
}
