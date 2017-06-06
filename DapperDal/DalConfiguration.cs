using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DapperDal.Implementor;
using DapperDal.Mapper;
using DapperDal.Sql;

namespace DapperDal
{
    public interface IDalConfiguration
    {
         Type DefaultMapper { get; set; }

         IList<Assembly> MappingAssemblies { get; set; }

         ISqlDialect Dialect { get; set; }

         IDalImplementor DapperImplementor { get; set; }

        /// <summary>
        /// 生成SQL时，是否添加 WITH (NOLOCK)
        /// </summary>
         bool Nolock { get; set; }

        /// <summary>
        /// SQL输出方法
        /// </summary>
         Action<string> OutputSql { get; set; }

        /// <summary>
        /// 实体集合返回前是否要缓冲（ToList）
        /// </summary>
         bool Buffered { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        void SetMappingAssemblies(IList<Assembly> assemblies);

        IClassMapper GetMap(Type entityType);

        IClassMapper GetMap<T>() where T : class;

        void ClearCache();

        Guid GetNextGuid();
    }

    public class DalConfiguration : IDalConfiguration
    {
        private readonly ConcurrentDictionary<Type, IClassMapper> _classMaps = new ConcurrentDictionary<Type, IClassMapper>();

        static DalConfiguration()
        {
            Default = new DalConfiguration();
        }

        public DalConfiguration()
            : this(typeof(AutoClassMapper<>), new List<Assembly>(), new SqlServerDialect())
        {
        }


        public DalConfiguration(Type defaultMapper, IList<Assembly> mappingAssemblies, ISqlDialect sqlDialect)
        {
            DefaultMapper = defaultMapper;
            MappingAssemblies = mappingAssemblies ?? new List<Assembly>();
            Dialect = sqlDialect;
            DapperImplementor = new DalImplementor(new SqlGeneratorImpl(this));
        }

        /// <summary>
        /// 全局默认配置
        /// </summary>
        public static IDalConfiguration Default { get; }

        public Type DefaultMapper { get; set; }

        public IList<Assembly> MappingAssemblies { get; set; }

        public ISqlDialect Dialect { get; set; }

        public IDalImplementor DapperImplementor { get; set; }

        /// <summary>
        /// 生成SQL时，是否添加 WITH (NOLOCK)
        /// </summary>
        public bool Nolock { get; set; }

        /// <summary>
        /// SQL输出方法
        /// </summary>
        public Action<string> OutputSql { get; set; }

        /// <summary>
        /// 实体集合返回前是否要缓冲（ToList）
        /// </summary>
        public bool Buffered { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblies"></param>
        public void SetMappingAssemblies(IList<Assembly> assemblies)
        {
            MappingAssemblies = assemblies ?? new List<Assembly>();
            ClearCache();
            DapperImplementor = new DalImplementor(new SqlGeneratorImpl(this));
        }

        public IClassMapper GetMap(Type entityType)
        {
            IClassMapper map;
            if (!_classMaps.TryGetValue(entityType, out map))
            {
                Type mapType = GetMapType(entityType);
                if (mapType == null)
                {
                    mapType = DefaultMapper.MakeGenericType(entityType);
                }

                map = Activator.CreateInstance(mapType) as IClassMapper;
                _classMaps[entityType] = map;
            }

            return map;
        }

        public IClassMapper GetMap<T>() where T : class
        {
            return GetMap(typeof(T));
        }

        public void ClearCache()
        {
            _classMaps.Clear();
        }

        public Guid GetNextGuid()
        {
            byte[] b = Guid.NewGuid().ToByteArray();
            DateTime dateTime = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan timeSpan = new TimeSpan(now.Ticks - dateTime.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes1 = BitConverter.GetBytes(timeSpan.Days);
            byte[] bytes2 = BitConverter.GetBytes((long)(timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes1);
            Array.Reverse(bytes2);
            Array.Copy(bytes1, bytes1.Length - 2, b, b.Length - 6, 2);
            Array.Copy(bytes2, bytes2.Length - 4, b, b.Length - 4, 4);
            return new Guid(b);
        }

        private Type GetMapType(Type entityType)
        {
            Func<Assembly, Type> getType = a =>
            {
                Type[] types = a.GetTypes();
                return (from type in types
                        let interfaceType = type.GetInterface(typeof(IClassMapper<>).FullName)
                        where
                            interfaceType != null &&
                            interfaceType.GetGenericArguments()[0] == entityType
                        select type).SingleOrDefault();
            };

            Type result = getType(entityType.Assembly);
            if (result != null)
            {
                return result;
            }

            foreach (var mappingAssembly in MappingAssemblies)
            {
                result = getType(mappingAssembly);
                if (result != null)
                {
                    return result;
                }
            }

            return getType(entityType.Assembly);
        }
    }
}