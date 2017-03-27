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
    /// <typeparam name="TPrimaryKey">实体ID（主键）类型</typeparam>
    public class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        static DalBase()
        {
            DapperExtensions.DapperExtensions.Configure(
                typeof(AutoEntityMapper<>), new List<Assembly>(), new SqlServerDialect());
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
        /// <param name="connectionName">DB连接字符串配置节点名</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="ConfigurationErrorsException">找不到配置节点</exception>
        public DalBase(string connectionName)
        {
            if (string.IsNullOrEmpty(connectionName))
            {
                throw new ArgumentNullException("connectionName");
            }

            var conStr = ConfigurationManager.ConnectionStrings[connectionName];
            if (conStr == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Failed to find connection string named '{0}' in app/web.config.", connectionName));
            }

            ConnectionString = conStr.ConnectionString;

            _connection = CreateConnection(ConnectionString);
        }

        /// <summary>
        /// 用DB连接初始化 DAL 新实例
        /// </summary>
        /// <param name="connection">DB连接</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        public DalBase(IDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            ConnectionString = connection.ConnectionString;

            _connection = connection;
        }

        /// <summary>
        /// DB连接字符串
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// DB连接实例
        /// </summary>
        private IDbConnection _connection;

        /// <summary>
        /// 获取DB连接实例
        /// </summary>
        public virtual IDbConnection Connection
        {
            get { return OpenConnection(); }
        }

        /// <summary>
        /// 创建DB连接
        /// </summary>
        /// <param name="connectionString">DB连接字符串</param>
        /// <returns>DB连接</returns>
        private IDbConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            var connection = new SqlConnection(connectionString);
            if (connection == null)
                throw new ConfigurationErrorsException(
                    string.Format("Failed to create a connection using the connection string '{0}'.", connectionString));

            connection.Open();

            return connection;
        }

        /// <summary>
        /// 打开DB连接
        /// </summary>
        /// <returns>DB连接</returns>
        protected virtual IDbConnection OpenConnection()
        {
            if (_connection == null)
            {
                _connection = CreateConnection(ConnectionString);
            }

            if (_connection.State != ConnectionState.Open)
            {
                if (string.IsNullOrEmpty(_connection.ConnectionString))
                {
                    _connection.ConnectionString = ConnectionString;
                }
                _connection.Open();
            }

            return _connection;
        }

        /// <summary>
        /// 插入指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体主键</returns>
        public virtual TPrimaryKey Insert(TEntity entity)
        {
            using (Connection)
            {
                return Connection.Insert(entity);
            }
        }

        /// <summary>
        /// 批量插入指定实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            using (Connection)
            {
                Connection.Insert(entities);
            }
        }

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(TEntity entity)
        {
            using (Connection)
            {
                return Connection.Delete(entity);
            }
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(object predicate)
        {
            using (Connection)
            {
                return Connection.Delete<TEntity>(predicate);
            }
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="predicate">删除条件</param>
        /// <returns>删除结果</returns>
        public virtual bool Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (Connection)
            {
                return Connection.Delete<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }

        /// <summary>
        /// 更新指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>更新结果</returns>
        public virtual bool Update(TEntity entity)
        {
            using (Connection)
            {
                return Connection.Update(entity);
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
            using (Connection)
            {
                return Connection.Update(entity, props.ToList());
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
            using (Connection)
            {
                return Connection.Update<TEntity>(entity);
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
            using (Connection)
            {
                return Connection.Update<TEntity>(entity, predicate);
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
            using (Connection)
            {
                return Connection.Update<TEntity>(entity, predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }

        /// <summary>
        /// 根据实体ID（主键）获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体</returns>
        public virtual TEntity Get(TPrimaryKey id)
        {
            using (Connection)
            {
                return Connection.Get<TEntity>(id);
            }
        }

        /// <summary>
        /// 获取所有实体列表
        /// </summary>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList()
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>();
            }
        }

        /// <summary>
        /// 根据查询条件获取实体列表
        /// （查询使用谓词或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(object predicate)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate);
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表
        /// （排序使用表达式）
        /// </summary>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(null,
                    sortingExpression.ToSortable(ascending));
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表
        /// （查询使用谓词或匿名对象，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(object predicate, object sort)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate, sort.ToSortable());
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表
        /// （查询使用谓词或匿名对象，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(object predicate,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate,
                    sortingExpression.ToSortable(ascending));
            }
        }

        /// <summary>
        /// 根据查询条件获取实体列表
        /// （查询使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表
        /// （查询使用表达式，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate, object sort)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sort.ToSortable());
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体列表
        /// （查询使用表达式，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体列表</returns>
        public virtual IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetList<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sortingExpression.ToSortable(ascending));
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页列表
        /// （查询使用谓词或匿名对象，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="pageNumber">页号，从1起始</param>
        /// <param name="itemsPerPage">每页条数</param>
        /// <returns>实体分页列表</returns>
        public virtual IEnumerable<TEntity> GetListPaged(object predicate, object sort,
            int pageNumber, int itemsPerPage)
        {
            using (Connection)
            {
                return Connection.GetPage<TEntity>(predicate,
                    sort.ToSortable(), pageNumber - 1, itemsPerPage).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页列表
        /// （查询使用谓词或匿名对象，排序表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pageNumber">页号，从1起始</param>
        /// <param name="itemsPerPage">每页条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体分页列表</returns>
        public virtual IEnumerable<TEntity> GetListPaged(object predicate,
            int pageNumber, int itemsPerPage,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetPage<TEntity>(predicate,
                    sortingExpression.ToSortable(ascending),
                    pageNumber - 1, itemsPerPage).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页列表
        /// （查询使用表达式，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="pageNumber">页号，从1起始</param>
        /// <param name="itemsPerPage">每页条数</param>
        /// <returns>实体分页列表</returns>
        public virtual IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, object sort,
            int pageNumber, int itemsPerPage)
        {
            using (Connection)
            {
                return Connection.GetPage<TEntity>(
                    predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sort.ToSortable(),
                    pageNumber - 1,
                    itemsPerPage).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页列表
        /// （查询使用表达式，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="pageNumber">页号，从1起始</param>
        /// <param name="itemsPerPage">每页条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体分页列表</returns>
        public virtual IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate,
            int pageNumber, int itemsPerPage,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetPage<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sortingExpression.ToSortable(ascending),
                    pageNumber - 1, itemsPerPage).ToList();
            }
        }


        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用谓词或匿名对象，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(object predicate, object sort,
            int firstResult, int maxResults)
        {
            using (Connection)
            {
                return Connection.GetSet<TEntity>(predicate, sort.ToSortable(),
                    firstResult, maxResults).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用谓词或匿名对象，排序表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(object predicate,
            int firstResult, int maxResults,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetSet<TEntity>(predicate,
                    sortingExpression.ToSortable(ascending),
                    firstResult, maxResults).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用表达式，排序使用Sort或匿名对象）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, object sort,
            int firstResult, int maxResults)
        {
            using (Connection)
            {
                return Connection.GetSet<TEntity>(
                    predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sort.ToSortable(),
                    firstResult, maxResults).ToList();
            }
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间列表
        /// （查询使用表达式，排序使用表达式）
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="ascending">排序方向</param>
        /// <param name="sortingExpression">排序字段</param>
        /// <returns>实体区间列表</returns>
        public virtual IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate,
            int firstResult, int maxResults,
            SortDirection ascending,
            params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            using (Connection)
            {
                return Connection.GetSet<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                    sortingExpression.ToSortable(ascending),
                    firstResult, maxResults).ToList();
            }
        }

        /// <summary>
        /// 根据条件获取实体条数
        /// （条件使用谓词或匿名对象）
        /// </summary>
        /// <param name="predicate">条件，使用谓词或匿名对象</param>
        /// <returns>实体条数</returns>
        public virtual int Count(object predicate)
        {
            using (Connection)
            {
                return Connection.Count<TEntity>(predicate);
            }
        }

        /// <summary>
        /// 根据条件获取实体条数
        /// （条件使用表达式）
        /// </summary>
        /// <param name="predicate">条件，使用表达式</param>
        /// <returns>实体条数</returns>
        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            using (Connection)
            {
                return Connection.Count<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>());
            }
        }

        /// <summary>
        /// 使用SQL语句获取实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TEntity> Query(string query)
        {
            using (Connection)
            {
                return Connection.Query<TEntity>(query);
            }
        }

        /// <summary>
        /// 使用SQL语句获取实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TEntity> Query(string query, object parameters)
        {
            using (Connection)
            {
                return Connection.Query<TEntity>(query, parameters);
            }
        }

        /// <summary>
        /// 使用SQL语句获取实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TEntity> Query(string query, object parameters, CommandType commandType)
        {
            using (Connection)
            {
                return Connection.Query<TEntity>(query, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// 使用SQL语句获取指定实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TAny> Query<TAny>(string query)
        {
            using (Connection)
            {
                return Connection.Query<TAny>(query);
            }
        }

        /// <summary>
        /// 使用SQL语句获取指定实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TAny> Query<TAny>(string query, object parameters)
        {
            using (Connection)
            {
                return Connection.Query<TAny>(query, parameters);
            }
        }

        /// <summary>
        /// 使用SQL语句获取指定实体集合
        /// </summary>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>实体集合</returns>
        public virtual IEnumerable<TAny> Query<TAny>(string query, object parameters, CommandType commandType)
        {
            using (Connection)
            {
                return Connection.Query<TAny>(query, parameters, commandType: commandType);
            }
        }
    }
}