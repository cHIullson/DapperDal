using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using DapperDal.Test.Entities;
using DapperExtensions;
using DapperExtensions.Expressions;
using NUnit.Framework;

namespace DapperDal.Test.IntegrationTests.SqlServer
{
    [TestFixture]
    public class CrudFixture
    {
        [TestFixture]
        public class InsertMethod : SqlServerBaseFixture
        {
            [Test]
            public void AddsEntityToDatabase_ReturnsKey()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p = new PersonEntity { PersonName = "Foo", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                int id = personDal.Insert(p);
                Assert.AreEqual(1, id);
                Assert.AreEqual(1, p.PersonId);
            }

            [Test]
            public void AddsMultipleEntitiesToDatabase()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Foo", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p3 = new PersonEntity { PersonName = "Baz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };

                personDal.Insert(new[] { p1, p2, p3 });

                var persons = personDal.GetList().ToList();
                Assert.AreEqual(3, persons.Count);
            }
        }

        [TestFixture]
        public class DeleteMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_DeletesFromDatabase()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now
                };
                int id = personDal.Insert(p1);

                PersonEntity p2 = personDal.Get(id);
                personDal.Delete(p2);
                Assert.IsNull(personDal.Get(id));
            }

            [Test]
            public void UsingPredicate_DeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                IPredicate pred = Predicates.Field<PersonEntity>(p => p.PersonName, Operator.Eq, "Bar");
                var result = personDal.Delete(pred);
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(1, list.Count());
            }

            [Test]
            public void UsingObject_DeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                var result = personDal.Delete(new { PersonName = "Bar" });
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(1, list.Count());
            }

            [Test]
            public void UsingExpression_DeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                var result = personDal.Delete(p => p.PersonName == "Bar");
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(1, list.Count());
            }

        }

        [TestFixture]
        public class SoftDeleteMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_SoftDeletesRows()
            {
                var personDal = new DalBase<PersonEntity, long>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 1
                };
                long id = personDal.Insert(p1);

                PersonEntity p2 = personDal.Get(id);
                personDal.SoftDeleteById(p2.PersonId);

                var p3 = personDal.Get(id);
                Assert.AreEqual(0, p3.IsActive);
            }


            [Test]
            public void UsingEntity_SoftDeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 1
                };
                int id = personDal.Insert(p1);

                PersonEntity p2 = personDal.Get(id);
                personDal.SoftDelete(p2);

                var p3 = personDal.Get(id);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingPredicate_SoftDeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                IPredicate pred = Predicates.Field<PersonEntity>(p => p.PersonName, Operator.Eq, "Bar");
                var result = personDal.SoftDelete(pred);
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(2, list.Count(d => d.IsActive == 0));
            }

            [Test]
            public void UsingObject_SoftDeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                var result = personDal.SoftDelete(new { PersonName = "Bar" });
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(2, list.Count(d => d.IsActive == 0));
            }

            [Test]
            public void UsingExpression_SoftDeletesRows()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p2 = new PersonEntity { PersonName = "Bar", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                PersonEntity p3 = new PersonEntity { PersonName = "Barz", CreateTime = DateTime.Now, UpdateTime = DateTime.Now, IsActive = 1 };
                personDal.Insert(p1);
                personDal.Insert(p2);
                personDal.Insert(p3);

                var list = personDal.GetList();
                Assert.AreEqual(3, list.Count());

                var result = personDal.SoftDelete(p => p.PersonName == "Bar");
                Assert.IsTrue(result);

                list = personDal.GetList();
                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(2, list.Count(d => d.IsActive == 0));
            }

        }

        [TestFixture]
        public class UpdateMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingEntity_UpdatesEntity()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.IsActive = 1;
                p2.PersonName = "Baz";

                personDal.Update(p2);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(1, p3.IsActive);
            }

            [Test]
            public void UsingEntity_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                personDal.Update(p2, new[] { "personName", "CarId", "CarName" });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingEntityProperties_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                personDal.Update(p2, new { personName = "Baz", CarId = 2 });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                var personName = p2.PersonName;
                personDal.Update(new { p2.PersonId, personName, p2.CarId, CarName = "CarName" });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WherePredicateKey_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                var predicate = Predicates.Field<PersonEntity>(f => f.PersonId, Operator.Eq, p2.PersonId);

                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WherePredicateProp_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                var predicate = Predicates.Field<PersonEntity>(f => f.PersonName, Operator.Eq, p1.PersonName);

                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WhereObjectKey_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    new { p2.PersonId });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WhereObjectProp_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    new { p1.PersonName });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WhereExpressionKey_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                Expression<Func<PersonEntity, bool>> predicate = p => p.PersonId == p2.PersonId;
                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }

            [Test]
            public void UsingObject_WhereExpressionProp_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = 1;

                Expression<Func<PersonEntity, bool>> predicate = p => p.PersonName == p1.PersonName;
                var personName = p2.PersonName;
                personDal.Update(new { personName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(0, p3.IsActive);
            }
        }

        [TestFixture]
        public class GetMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_ReturnsEntity()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = 0
                };
                int id = personDal.Insert(p1);

                PersonEntity p2 = personDal.Get(id);
                Assert.AreEqual(id, p2.PersonId);
                Assert.AreEqual("Bar", p2.PersonName);
            }

        }

        [TestFixture]
        public class GetFirstMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsFirst()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                PersonEntity p2 = personDal.GetFirst();
                Assert.AreEqual(1, p2.CarId);
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsOrderedFirst()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                PersonEntity p2 = personDal.GetFirst(SortDirection.Descending, sort);

                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                PersonEntity p2 = personDal.GetFirst(predicate);

                Assert.AreEqual(1, p2.CarId);
            }

            [Test]
            public void UsingNullPredicate_BySort_ReturnsOrderedFirst()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };
                PersonEntity p2 = personDal.GetFirst((object)null, sort);

                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                PersonEntity p2 = personDal.GetFirst(predicate, sort);

                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                PersonEntity p2 = personDal.GetFirst(predicate, SortDirection.Descending, sort);
                Assert.AreEqual(3, p2.CarId);
            }


            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                PersonEntity p2 = personDal.GetFirst(predicate);
                Assert.AreEqual(1, p2.CarId);
            }

            [Test]
            public void UsingNullPredicate_ByObject_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new { CarId = SortDirection.Descending };
                PersonEntity p2 = personDal.GetFirst((object)null, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                var sort = new { CarId = SortDirection.Descending };
                PersonEntity p2 = personDal.GetFirst(predicate, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                PersonEntity p2 = personDal.GetFirst(predicate, SortDirection.Descending, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingExpression_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                PersonEntity p2 = personDal.GetFirst(p => p.IsActive == 1);
                Assert.AreEqual(1, p2.CarId);
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                PersonEntity p2 = personDal.GetFirst(predicate, SortDirection.Descending, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingExpression_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                PersonEntity p2 = personDal.GetFirst(predicate, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new { CarId = SortDirection.Descending };
                PersonEntity p2 = personDal.GetFirst(predicate, sort);
                Assert.AreEqual(3, p2.CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                PersonEntity p2 = personDal.GetFirst(predicate, SortDirection.Descending, sort);
                Assert.AreEqual(3, p2.CarId);
            }

        }

        [TestFixture]
        public class GetListMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsAll()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.GetList();
                Assert.AreEqual(4, list.Count());
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsAllOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(SortDirection.Descending, sort);

                Assert.AreEqual(4, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "a" || p.PersonName == "c"));
            }

            [Test]
            public void UsingNullPredicate_BySort_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };
                IEnumerable<PersonEntity> list = personDal.GetList((object)null, sort).ToList();

                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }


            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1, PersonName = "c" };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "c"));
            }

            [Test]
            public void UsingNullPredicate_ByObject_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList((object)null, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => true;
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(4, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.IsActive == 1 && p.PersonName == "c";
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.IsActive != 1 && p.PersonName == "b";
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.PersonName != "b";
                list = personDal.GetList(p => p.PersonName != "b");
                Assert.AreEqual(3, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.IsActive == 1 && p.PersonName == "c" && p.CarId == 3;
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.IsActive == 1 && p.PersonName == "c" || p.CarId == 3;
                list = personDal.GetList(predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));

                predicate = p => p.IsActive == 1 && (p.PersonName == "c" || p.CarId == 3);
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(predicate.Compile()));
            }

            [Test]
            public void UsingCombineExpression_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => true;
                predicate = PredicateBuilder.True<PersonEntity>();
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(4, list.Count());
                Assert.IsTrue(list.All(p => true));

                predicate = p => p.IsActive == 1 && p.PersonName == "c";
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.IsActive == 1);
                predicate = predicate.And(p => p.PersonName == "c");
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1 && p.PersonName == "c"));

                predicate = p => p.IsActive != 1 && p.PersonName == "b";
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.IsActive != 1);
                predicate = predicate.And(p => p.PersonName == "b");
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive != 1 && p.PersonName == "b"));

                predicate = p => p.PersonName != "b";
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.PersonName != "b");
                list = personDal.GetList(predicate);
                Assert.AreEqual(3, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName != "b"));

                predicate = p => p.IsActive == 1 && p.PersonName == "c" && p.CarId == 3;
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.IsActive == 1);
                predicate = predicate.And(p => p.PersonName == "c");
                predicate = predicate.And(p => p.CarId == 3);
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1 && p.PersonName == "c" && p.CarId == 3));

                predicate = p => p.IsActive == 1 && p.PersonName == "c" || p.CarId == 3;
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.IsActive == 1);
                predicate = predicate.And(p => p.PersonName == "c");
                predicate = predicate.Or(p => p.CarId == 3);
                list = personDal.GetList(predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1 && p.PersonName == "c" || p.CarId == 3));

                predicate = p => p.IsActive == 1 && (p.PersonName == "c" || p.CarId == 3);
                predicate = PredicateBuilder.True<PersonEntity>();
                predicate = predicate.And(p => p.IsActive == 1);
                var predicate2 = PredicateBuilder.False<PersonEntity>().Or(p => p.PersonName == "c").Or(p => p.CarId == 3);
                predicate = predicate.And(predicate2);
                list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1 && (p.PersonName == "c" || p.CarId == 3)));
            }


            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();

                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

        }

        [TestFixture]
        public class GetPageMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetListPaged(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, 2, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetListPaged(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, 2, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetListPaged(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, 2, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

        }

        [TestFixture]
        public class GetSetMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

        }

        [TestFixture]
        public class GetTopMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsLimit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.GetTop(3);
                Assert.AreEqual(3, list.Count());
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsLimitOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetTop(3, SortDirection.Descending, sort);

                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1));
            }

            [Test]
            public void UsingNullPredicate_BySort_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };
                IEnumerable<PersonEntity> list = personDal.GetTop(3, (object)null, sort).ToList();

                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }


            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.IsActive == 1));
            }

            [Test]
            public void UsingNullPredicate_ByObject_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetTop(3, (object)null, sort).ToList();
                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.GetTop(2, p => p.IsActive == 1);
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(1, list.First().CarId);
                Assert.AreEqual(3, list.Last().CarId);
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetTop(3, predicate, SortDirection.Descending, sort).ToList();

                Assert.AreEqual(3, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetTop(2, predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(2, list.Last().CarId);
            }

        }

        [TestFixture]
        public class CountMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNull_Returns_NoCount()
            {
                var personDal = new DalBase<PersonEntity>();
                Assert.AreEqual(0, personDal.Count(null));
                Assert.AreEqual(0, personDal.Count());
            }

            [Test]
            public void UsingNull_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(4, personDal.Count(null));
                Assert.AreEqual(4, personDal.Count());
            }

            [Test]
            public void UsingPredicate_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(4, personDal.Count(null));
                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Assert.AreEqual(2, personDal.Count(predicate));
            }

            [Test]
            public void UsingObject_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Assert.AreEqual(2, personDal.Count(predicate));
            }

            [Test]
            public void UsingNullExpression_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Assert.AreEqual(4, personDal.Count(predicate));
            }


            [Test]
            public void UsingExpression_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Assert.AreEqual(2, personDal.Count(predicate));
            }

        }

        [TestFixture]
        public class ExsitMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNull_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(true, personDal.Exsit());
                var id = 1;
                Assert.AreEqual(true, personDal.Exsit(id));
            }

            [Test]
            public void UsingNull_Returns_NotExsit()
            {
                var personDal = new DalBase<PersonEntity>();


                Assert.AreEqual(false, personDal.Exsit());
                var id = 1;
                Assert.AreEqual(false, personDal.Exsit(id));
            }


            [Test]
            public void UsingId_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(true, personDal.Exsit(null));
                var id = 1;
                Assert.AreEqual(true, personDal.Exsit(id));
            }

            [Test]
            public void UsingPredicate_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(true, personDal.Exsit(null));
                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, 1);
                Assert.AreEqual(true, personDal.Exsit(predicate));
            }

            [Test]
            public void UsingObject_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = 1 };
                Assert.AreEqual(true, personDal.Exsit(predicate));
            }

            [Test]
            public void UsingNullExpression_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Assert.AreEqual(true, personDal.Exsit(predicate));
            }


            [Test]
            public void UsingExpression_Returns_Exsit()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == 1;
                Assert.AreEqual(true, personDal.Exsit(predicate));
            }

        }

        class PersonModel
        {
            public string Name { get; set; }

            public int CarId { get; set; }
        }


        [TestFixture]
        public class QueryMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNone_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "select * from Person where CarId = 3");

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "select * from Person where CarId = @CarId", new { CarId = 3 });

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_WithProcedure_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "P_GetPersonsByCarId", new { CarId = 3 }, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingNone_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "select PersonName as Name, CarId from Person where CarId = 3");

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "select PersonName as Name, CarId from Person where CarId = @CarId", new { CarId = 3 });

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_WithProcedure_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "dbo.P_GetPersonModelsByCarId",
                    new { CarId = 3 }, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingDynamicParameter_WithProcedure_ReturnsModels_OutputCount()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var parameters = new DynamicParameters(new { CarId = 3 });
                parameters.Add("TotalCount", dbType: System.Data.DbType.Int32, direction: System.Data.ParameterDirection.Output);

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "dbo.P_GetPersonModelsByCarId_OutputCount",
                    parameters, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(2, parameters.Get<int>("TotalCount"));
                Assert.AreEqual(2, parameters.Get<object>("TotalCount"));
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }

            class PersonParam
            {
                public int CarId { get; set; }
                public int TotalCount { get; set; }

            }

            [Test]
            public void UsingTypeParameter_WithProcedure_ReturnsModels_OutputCount()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var p = new PersonParam { CarId = 3, TotalCount = 0 };
                var parameters = new DynamicParameters(p)
                    .Output(p, d => d.TotalCount, dbType: System.Data.DbType.Int32);
                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "dbo.P_GetPersonModelsByCarId_OutputCount",
                    parameters, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
                Assert.AreEqual(2, parameters.Get<int>("TotalCount"));
                Assert.AreEqual(2, parameters.Get<object>("TotalCount"));
                Assert.AreEqual(2, p.TotalCount);
                Assert.IsTrue(list.All(d => d.CarId == 3));
            }
        }

        [TestFixture]
        public class QueryMultipleMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNone_ReturnsMultiples()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var tuple = personDal.QueryMultiple<PersonEntity, PersonModel>(
                    "select * from Person where CarId = 3;select PersonName AS [Name], CarId from Person where CarId = 3");

                Assert.AreEqual(2, tuple.Item1.Count());
                Assert.IsTrue(tuple.Item1.All(d => d.CarId == 3));
                Assert.AreEqual(2, tuple.Item2.Count());
                Assert.IsTrue(tuple.Item2.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_ReturnsMultiples()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var tuple = personDal.QueryMultiple<PersonEntity, PersonModel>(
                    "select * from Person where CarId = @CarId;select PersonName AS [Name], CarId from Person where CarId = @CarId",
                    new { CarId = 3 });

                Assert.AreEqual(2, tuple.Item1.Count());
                Assert.IsTrue(tuple.Item1.All(d => d.CarId == 3));
                Assert.AreEqual(2, tuple.Item2.Count());
                Assert.IsTrue(tuple.Item2.All(d => d.CarId == 3));
            }

            [Test]
            public void UsingParameter_WithProcedure_ReturnsMultiples()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = 1, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var tuple = personDal.QueryMultiple<PersonEntity, PersonModel>(
                    "P_GetPersonMultipleModelsByCarId", new { CarId = 3 }, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, tuple.Item1.Count());
                Assert.IsTrue(tuple.Item1.All(d => d.CarId == 3));
                Assert.AreEqual(2, tuple.Item2.Count());
                Assert.IsTrue(tuple.Item2.All(d => d.CarId == 3));
            }
        }
    }
}
