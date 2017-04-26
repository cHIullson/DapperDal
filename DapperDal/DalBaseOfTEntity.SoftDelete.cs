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
        /// 逻辑删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="props">逻辑删除属性名及更新值，默认 IsActive=0</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(TEntity entity, object props = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.SoftDelete(entity, props);
            }
        }

        /// <summary>
        /// 根据实体主键ID逻辑删除指定实体
        /// </summary>
        /// <param name="id">实体</param>
        /// <param name="props">逻辑删除属性名及更新值，默认 IsActive=0</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDeleteById(TPrimaryKey id, object props = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.SoftDeleteById<TEntity>(id, props);
            }
        }

        /// <summary>
        /// 根据条件逻辑删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <param name="props">逻辑删除属性名及更新值，默认 IsActive=0</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(object predicate, object props = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.SoftDelete<TEntity>(predicate, props);
            }
        }

        /// <summary>
        /// 根据条件逻辑删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <param name="props">逻辑删除属性名及更新值，默认 IsActive=0 }</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(Expression<Func<TEntity, bool>> predicate, object props = null)
        {
            using (var connection = OpenConnection())
            {
                return connection.SoftDelete<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), props);
            }
        }

    }
}
