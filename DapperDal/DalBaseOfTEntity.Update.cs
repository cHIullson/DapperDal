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
        /// 更新指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(TEntity entity)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update(entity);
            }
        }

        /// <summary>
        /// 更新指定实体指定属性
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="props">更新属性名</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(TEntity entity, IEnumerable<string> props)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update(entity, props.ToList());
            }
        }

        /// <summary>
        /// 更新指定实体指定属性
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="props">更新属性名</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(TEntity entity, object props)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update(entity, props);
            }
        }

        /// <summary>
        /// 根据指定指定主键ID更新实体指定属性
        /// （条件使用实体主键ID）
        /// </summary>
        /// <param name="entity">更新实体，包含主键ID与更新属性</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(object entity)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update<TEntity>(entity);
            }
        }

        /// <summary>
        /// 根据指定更新条件更新实体指定属性
        /// （条件使用谓词或匿名对象）
        /// </summary>
        /// <param name="entity">更新属性</param>
        /// <param name="predicate">更新条件，使用谓词或匿名对象</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(object entity, object predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update<TEntity>(entity, predicate);
            }
        }

        /// <summary>
        /// 根据指定更新条件更新实体指定属性
        /// （条件使用表达式）
        /// </summary>
        /// <param name="entity">更新属性</param>
        /// <param name="predicate">更新条件，使用表达式</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(object entity, Expression<Func<TEntity, bool>> predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Update<TEntity>(entity, predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }
    }
}
