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
        /// 删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(TEntity entity)
        {
            using (var connection = OpenConnection())
            {
                return connection.Delete(entity);
            }
        }

        /// <summary>
        /// 根据实体主键ID删除指定实体
        /// </summary>
        /// <param name="id">实体主键ID</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(TPrimaryKey id)
        {
            using (var connection = OpenConnection())
            {
                IPredicate predicate = PredicateExtensions.GetIdPredicate<TEntity>(id);

                return connection.Delete<TEntity>(predicate);
            }
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(object predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Delete<TEntity>(predicate);
            }
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Delete<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }
    }
}
