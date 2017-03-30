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

        /// <summary>
        /// 逻辑删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="props">逻辑删除属性名及更新值，默认 IsActive=0</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(TEntity entity, object props = null)
        {
            if (props == null)
            {
                props = new { IsActive = 0 };
            }
            return Update(entity, props);
        }

        /// <summary>
        /// 根据条件逻辑删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <param name="entity">逻辑删除属性名及更新值，默认 IsActive=0</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(object predicate, object entity = null)
        {
            if (entity == null)
            {
                entity = new { IsActive = 0 };
            }
            return Update(entity, predicate);
        }

        /// <summary>
        /// 根据条件逻辑删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <param name="entity">逻辑删除属性名及更新值，默认 IsActive=0 }</param>
        /// <returns>逻辑删除结果</returns>
        public virtual bool SoftDelete(Expression<Func<TEntity, bool>> predicate, object entity = null)
        {
            if (entity == null)
            {
                entity = new { IsActive = 0 };
            }
            return Update(entity, predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
        }

    }
}
