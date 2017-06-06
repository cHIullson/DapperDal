
using DapperDal.Implementor;
using DapperDal.Mapper;
using DapperDal.Predicate;

namespace DapperDal.Extensions
{
    /// <summary>
    /// 谓词生成帮助方法
    /// </summary>
    public static class PredicateHelper
    {
        internal static DalImplementor Instance
        {
            get
            {
                return DalConfiguration.Default.DapperImplementor as DalImplementor;
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
            IClassMapper classMap = DalConfiguration.Default.GetMap<T>();
            IPredicate predicate = Instance.GetIdPredicate(classMap, id);
            return predicate;
        }

    }
}
