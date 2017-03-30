using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;

namespace DapperDal
{
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>
            Query<TFirst, TSecond>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>
            Query<TFirst, TSecond>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>
            Query<TFirst, TSecond>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>
            Query<TFirst, TSecond, TThird>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>
            Query<TFirst, TSecond, TThird>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>
            Query<TFirst, TSecond, TThird>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>
            Query<TFirst, TSecond, TThird, TFourth>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>
            Query<TFirst, TSecond, TThird, TFourth>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>>
            Query<TFirst, TSecond, TThird, TFourth>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <typeparam name="TEighth">第八个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>, Tuple<IEnumerable<TEighth>>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth>(string query)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList(),
                        (IEnumerable<TEighth>)gridReader.Read<TEighth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <typeparam name="TEighth">第八个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>, Tuple<IEnumerable<TEighth>>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth>(string query, object parameters)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList(),
                        (IEnumerable<TEighth>)gridReader.Read<TEighth>().ToList());
                }
            }
        }

        /// <summary>
        /// 使用SQL语句获取多个指定实体集合
        /// </summary>
        /// <typeparam name="TFirst">第一个实体类型</typeparam>
        /// <typeparam name="TSecond">第二个实体类型</typeparam>
        /// <typeparam name="TThird">第三个实体类型</typeparam>
        /// <typeparam name="TFourth">第四个实体类型</typeparam>
        /// <typeparam name="TFifth">第五个实体类型</typeparam>
        /// <typeparam name="TSixth">第六个实体类型</typeparam>
        /// <typeparam name="TSeventh">第七个实体类型</typeparam>
        /// <typeparam name="TEighth">第八个实体类型</typeparam>
        /// <param name="query">SQL语句</param>
        /// <param name="parameters">SQL参数</param>
        /// <param name="commandType">SQL语句命令类型</param>
        /// <returns>多个实体集合</returns>
        public virtual Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>, IEnumerable<TFourth>, IEnumerable<TFifth>, IEnumerable<TSixth>, IEnumerable<TSeventh>, Tuple<IEnumerable<TEighth>>>
            Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TEighth>(string query, object parameters, CommandType commandType)
        {
            using (var connection = OpenConnection())
            {
                using (var gridReader = connection.QueryMultiple(query, parameters, commandType: commandType))
                {
                    return Tuple.Create((IEnumerable<TFirst>)gridReader.Read<TFirst>().ToList(),
                        (IEnumerable<TSecond>)gridReader.Read<TSecond>().ToList(),
                        (IEnumerable<TThird>)gridReader.Read<TThird>().ToList(),
                        (IEnumerable<TFourth>)gridReader.Read<TFourth>().ToList(),
                        (IEnumerable<TFifth>)gridReader.Read<TFifth>().ToList(),
                        (IEnumerable<TSixth>)gridReader.Read<TSixth>().ToList(),
                        (IEnumerable<TSeventh>)gridReader.Read<TSeventh>().ToList(),
                        (IEnumerable<TEighth>)gridReader.Read<TEighth>().ToList());
                }
            }
        }
    }
}
