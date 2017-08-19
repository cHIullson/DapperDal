using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using DapperDal.Mapper;
using DapperDal.Test.Entities;
using DapperDal.Test.Maps;
using NUnit.Framework;

namespace DapperDal.Test.IntegrationTests
{
    [TestFixture]
    public class NonCrudFixture
    {
        [TestFixture]
        public class GetNextGuidMethod
        {
            [Test]
            public void GetMultiple_DoesNotDuplicate()
            {
                List<Guid> list = new List<Guid>();
                for (int i = 0; i < 1000; i++)
                {
                    Guid id = DalConfiguration.Default.GetNextGuid();
                    Assert.IsFalse(list.Contains(id));
                    list.Add(id);
                }
            }
        }

        [TestFixture]
        public class GetMapMethod
        {
            [Test]
            public void NoMappingClass_ReturnsDefaultMapper()
            {
                var mapper = DalConfiguration.Default.GetMap<EntityWithoutMapper>();
                Assert.AreEqual(typeof(AutoClassMapper<EntityWithoutMapper>), mapper.GetType());
            }

            [Test]
            public void ClassMapperDescendant_Returns_DefinedClass()
            {
                var mapper = DalConfiguration.Default.GetMap<EntityWithMapper>();
                Assert.AreEqual(typeof(EntityWithMapperMapper), mapper.GetType());
            }

            [Test]
            public void ClassMapperInterface_Returns_DefinedMapper()
            {
                var mapper = DalConfiguration.Default.GetMap<EntityWithInterfaceMapper>();
                Assert.AreEqual(typeof(EntityWithInterfaceMapperMapper), mapper.GetType());
            }

            [Test]
            public void MappingClass_ReturnsFromDifferentAssembly()
            {
                DalConfiguration.Default.SetMappingAssemblies(new[] { typeof(ExternallyMappedMap).Assembly });
                var mapper = DalConfiguration.Default.GetMap<ExternallyMapped>();
                Assert.AreEqual(typeof(ExternallyMappedMap.ExternallyMappedMapper), mapper.GetType());

                DalConfiguration.Default.SetMappingAssemblies(null);
                mapper = DalConfiguration.Default.GetMap<ExternallyMapped>();
                Assert.AreEqual(typeof(AutoClassMapper<ExternallyMapped>), mapper.GetType());
            }

            private class EntityWithoutMapper
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }

            private class EntityWithMapper
            {
                public string Key { get; set; }
                public string Value { get; set; }
            }

            private class EntityWithMapperMapper : ClassMapper<EntityWithMapper>
            {
                public EntityWithMapperMapper()
                {
                    Map(p => p.Key).Column("EntityKey").Key(KeyType.Assigned);
                    AutoMap();
                }
            }

            private class EntityWithInterfaceMapper
            {
                public string Key { get; set; }
                public string Value { get; set; }
            }

            private class EntityWithInterfaceMapperMapper : IClassMapper<EntityWithInterfaceMapper>
            {
                public string SchemaName { get; private set; }
                public string TableName { get; private set; }
                public IList<IPropertyMap> Properties { get; private set; }
                public Type EntityType { get; private set; }

                public PropertyMap Map(Expression<Func<EntityWithInterfaceMapper, object>> expression)
                {
                    throw new NotImplementedException();
                }

                public PropertyMap Map(PropertyInfo propertyInfo)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}