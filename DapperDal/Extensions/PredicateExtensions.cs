
using DapperDal.Implementor;
using DapperDal.Mapper;

namespace DapperDal.Extensions
{
    public static class PredicateHelper
    {
        internal static DapperImplementor Instance
        {
            get
            {
                return DapperConfiguration.Default.DapperImplementor as DapperImplementor;
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
            IClassMapper classMap = DapperConfiguration.Default.GetMap<T>();
            IPredicate predicate = Instance.GetIdPredicate(classMap, id);
            return predicate;
        }

    }
}
