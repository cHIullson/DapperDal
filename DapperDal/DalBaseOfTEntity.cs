using System;
using System.Configuration;

namespace DapperDal
{
    /// <summary>
    /// 实体数据访问层基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class DalBase<TEntity> : DalBase<TEntity, int> where TEntity : class
    {
        /// <summary>
        /// 默认初始化 DAL 新实例
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// 用配置节点名初始化 DAL 新实例
        /// </summary>
        /// <param name="connectionName">DB连接字符串配置节点名</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="ConfigurationErrorsException">找不到配置节点</exception>
        public DalBase(string connectionName) : base(connectionName)
        {
        }
    }
}
