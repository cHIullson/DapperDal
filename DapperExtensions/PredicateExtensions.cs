using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperExtensions.Mapper;

namespace DapperExtensions
{
    public static class PredicateExtensions
    {
        internal static DapperImplementor Instance
        {
            get
            {
                return DapperExtensions.Instance as DapperImplementor;
            }
        }

        /// <summary>
        /// 获取实体主键ID条件谓词
        /// </summary>
        /// <param name="id">实体主键ID</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>谓词</returns>
        public static IPredicate GetIdPredicate<T>(object id) where T : class
        {
            IClassMapper classMap = Instance.SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = Instance.GetIdPredicate(classMap, id);
            return predicate;
        }

    }
}
