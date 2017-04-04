using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DapperExtensions.Sql;
using DapperExtensions.Mapper;

namespace DapperExtensions
{
    public static class DapperExtensions
    {
        private static readonly object _lock = new object();

        private static Func<IDapperExtensionsConfiguration, IDapperImplementor> _instanceFactory;
        private static IDapperImplementor _instance;
        private static IDapperExtensionsConfiguration _configuration;

        /// <summary>
        /// Gets or sets the default class mapper to use when generating class maps. If not specified, AutoClassMapper<T> is used.
        /// DapperExtensions.Configure(Type, IList<Assembly>, ISqlDialect) can be used instead to set all values at once
        /// </summary>
        public static Type DefaultMapper
        {
            get
            {
                return _configuration.DefaultMapper;
            }
        }

        /// <summary>
        /// Gets or sets the type of sql to be generated.
        /// DapperExtensions.Configure(Type, IList<Assembly>, ISqlDialect) can be used instead to set all values at once
        /// </summary>
        public static ISqlDialect SqlDialect
        {
            get
            {
                return _configuration.Dialect;
            }
        }

        /// <summary>
        /// Get or sets the Dapper Extensions Implementation Factory.
        /// </summary>
        public static Func<IDapperExtensionsConfiguration, IDapperImplementor> InstanceFactory
        {
            get
            {
                if (_instanceFactory == null)
                {
                    _instanceFactory = config => new DapperImplementor(new SqlGeneratorImpl(config));
                }

                return _instanceFactory;
            }
        }

        /// <summary>
        /// Gets the Dapper Extensions Implementation
        /// </summary>
        private static IDapperImplementor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = InstanceFactory(_configuration);
                        }
                    }
                }

                return _instance;
            }
        }

        static DapperExtensions()
        {
            Configure(new DapperExtensionsConfiguration(typeof(AutoClassMapper<>), new List<Assembly>(),
                new SqlServerDialect()));
        }

        /// <summary>
        /// Add other assemblies that Dapper Extensions will search if a mapping is not found in the same assembly of the POCO.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void SetMappingAssemblies(IList<Assembly> assemblies)
        {
            Configure(_configuration.DefaultMapper, assemblies, _configuration.Dialect);
        }

        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        /// <param name="defaultMapper"></param>
        /// <param name="mappingAssemblies"></param>
        /// <param name="sqlDialect"></param>
        private static void Configure(IDapperExtensionsConfiguration configuration)
        {
            _instance = null;
            _configuration = configuration;
        }

        /// <summary>
        /// Configure DapperExtensions extension methods.
        /// </summary>
        /// <param name="defaultMapper"></param>
        /// <param name="mappingAssemblies"></param>
        /// <param name="sqlDialect"></param>
        private static void Configure(Type defaultMapper, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            Configure(new DapperExtensionsConfiguration(defaultMapper, mappingAssemblies, sqlDialect));
        }

        /// <summary>
        /// 配置 DapperExtensionsConfiguration
        /// </summary>
        /// <param name="configure">配置方法</param>
        public static void Configure(Action<IDapperExtensionsConfiguration> configure)
        {
            if (configure != null && _configuration != null)
            {
                configure(_configuration);
            }
        }


        /// <summary>
        /// Executes a query for the specified id, returning the data typed as per T
        /// </summary>
        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var result = Instance.Get<T>(connection, id, transaction, commandTimeout);
            return (T)result;
        }

        /// <summary>
        /// Executes an insert query for the specified entity.
        /// </summary>
        public static void Insert<T>(this IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Instance.Insert<T>(connection, entities, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an insert query for the specified entity, returning the primary key.  
        /// If the entity has a single key, just the value is returned.  
        /// If the entity has a composite key, an IDictionary&lt;string, object&gt; is returned with the key values.
        /// The key value for the entity will also be updated if the KeyType is a Guid or Identity.
        /// </summary>
        public static dynamic Insert<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Insert<T>(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Update<T>(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, T entity, IEnumerable<string> props, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Update<T>(connection, entity, props.ToList(), transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, T entity, object props, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Update<T>(connection, entity, props, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, object keyAndProps, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Update<T>(connection, keyAndProps, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes an update query for the specified entity and predicate.
        /// </summary>
        public static bool Update<T>(this IDbConnection connection, object props, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Update<T>(connection, props, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// 逻辑删除指定实体
        /// </summary>
        public static bool SoftDelete<T>(this IDbConnection connection, T entity, object props, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (props == null && _configuration.SoftDeletePropsFactory != null)
            {
                props = _configuration.SoftDeletePropsFactory();
            }

            return Instance.Update<T>(connection, entity, props, transaction, commandTimeout);
        }

        /// <summary>
        /// 根据条件逻辑删除实体
        /// </summary>
        public static bool SoftDelete<T>(this IDbConnection connection, object predicate, object props, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (props == null && _configuration.SoftDeletePropsFactory != null)
            {
                props = _configuration.SoftDeletePropsFactory();
            }

            return Instance.Update<T>(connection, props, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a delete query for the specified entity.
        /// </summary>
        public static bool Delete<T>(this IDbConnection connection, T entity, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Delete<T>(connection, entity, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a delete query using the specified predicate.
        /// </summary>
        public static bool Delete<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Delete<T>(connection, predicate, transaction, commandTimeout);
        }

        public static IEnumerable<T> GetTop<T>(this IDbConnection connection, int limit, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, bool? buffered = null) where T : class
        {
            return Instance.GetTop<T>(connection, limit, predicate, sort, transaction, commandTimeout, buffered ?? _configuration.Buffered);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// </summary>
        public static IEnumerable<T> GetList<T>(this IDbConnection connection, object predicate = null, IList<ISort> sort = null, IDbTransaction transaction = null, int? commandTimeout = null, bool? buffered = null) where T : class
        {
            return Instance.GetList<T>(connection, predicate, sort, transaction, commandTimeout, buffered ?? _configuration.Buffered);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified page and resultsPerPage.
        /// </summary>
        public static IEnumerable<T> GetPage<T>(this IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction = null, int? commandTimeout = null, bool? buffered = null) where T : class
        {
            return Instance.GetPage<T>(connection, predicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered ?? _configuration.Buffered);
        }

        /// <summary>
        /// Executes a select query using the specified predicate, returning an IEnumerable data typed as per T.
        /// Data returned is dependent upon the specified firstResult and maxResults.
        /// </summary>
        public static IEnumerable<T> GetSet<T>(this IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction = null, int? commandTimeout = null, bool? buffered = null) where T : class
        {
            return Instance.GetSet<T>(connection, predicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered ?? _configuration.Buffered);
        }

        /// <summary>
        /// Executes a query using the specified predicate, returning an integer that represents the number of rows that match the query.
        /// </summary>
        public static int Count<T>(this IDbConnection connection, object predicate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return Instance.Count<T>(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Executes a select query for multiple objects, returning IMultipleResultReader for each predicate.
        /// </summary>
        public static IMultipleResultReader GetMultiple(this IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return Instance.GetMultiple(connection, predicate, transaction, commandTimeout);
        }

        /// <summary>
        /// Gets the appropriate mapper for the specified type T. 
        /// If the mapper for the type is not yet created, a new mapper is generated from the mapper type specifed by DefaultMapper.
        /// </summary>
        public static IClassMapper GetMap<T>() where T : class
        {
            return Instance.SqlGenerator.Configuration.GetMap<T>();
        }

        /// <summary>
        /// Clears the ClassMappers for each type.
        /// </summary>
        public static void ClearCache()
        {
            Instance.SqlGenerator.Configuration.ClearCache();
        }

        /// <summary>
        /// Generates a COMB Guid which solves the fragmented index issue.
        /// See: http://davybrion.com/blog/2009/05/using-the-guidcomb-identifier-strategy
        /// </summary>
        public static Guid GetNextGuid()
        {
            return Instance.SqlGenerator.Configuration.GetNextGuid();
        }
    }
}
