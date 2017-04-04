using Dapper;
using DapperDal.Mapper;
using DapperExtensions;
using DapperExtensions.Expressions;
using DapperExtensions.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

    /// <summary>
    /// 实体数据访问层基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">实体ID（主键）类型</typeparam>
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        static DalBase()
        {
            DapperExtensions.DapperExtensions.Configure(
                configuration =>
                {
                    configuration.DefaultMapper = typeof(AutoEntityMapper<>);
                    configuration.Nolock = true;
                    configuration.SoftDeletePropsFactory = () => new { IsActive = 0 };
                    configuration.Buffered = true;
                });
        }

        /// <summary>
        /// 默认初始化 DAL 新实例
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// 用配置节点名初始化 DAL 新实例
        /// </summary>
        /// <param name="connNameOrConnStr">DB连接字符串配置节点名</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="ConfigurationErrorsException">找不到配置节点</exception>
        public DalBase(string connNameOrConnStr)
        {
            if (string.IsNullOrEmpty(connNameOrConnStr))
            {
                throw new ArgumentNullException("connNameOrConnStr");
            }

            if (connNameOrConnStr.Contains("=") || connNameOrConnStr.Contains(";"))
            {
                ConnectionString = connNameOrConnStr;
            }
            else
            {
                var conStr = ConfigurationManager.ConnectionStrings[connNameOrConnStr];
                if (conStr == null)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Failed to find connection string named '{0}' in app/web.config.", connNameOrConnStr));
                }

                ConnectionString = conStr.ConnectionString;
            }
        }

        /// <summary>
        /// DB连接字符串
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// 打开DB连接
        /// </summary>
        /// <returns>DB连接</returns>
        protected virtual IDbConnection OpenConnection()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var connection = new SqlConnection(ConnectionString);
            if (connection == null)
                throw new ConfigurationErrorsException(
                    string.Format("Failed to create a connection using the connection string '{0}'.", ConnectionString));

            connection.Open();

            return connection;
        }
    }
}