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
        /// 判断指定条件的实体是否存在
        /// （条件使用谓词或匿名对象）
        /// </summary>
        /// <param name="predicate">条件，使用谓词或匿名对象</param>
        /// <returns>实体条数</returns>
        public virtual bool Exsit(object predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Count<TEntity>(predicate) > 0;
            }
        }

        /// <summary>
        /// 判断指定条件的实体是否存在
        /// （条件使用表达式）
        /// </summary>
        /// <param name="predicate">条件，使用表达式</param>
        /// <returns>实体条数</returns>
        public virtual bool Exsit(Expression<Func<TEntity, bool>> predicate)
        {
            using (var connection = OpenConnection())
            {
                return connection.Count<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>()) > 0;
            }
        }
    }
}
