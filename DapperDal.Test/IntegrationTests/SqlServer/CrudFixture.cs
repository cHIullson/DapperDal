using DapperDal.Test.Entities;
using DapperExtensions;
using DapperExtensions.Expressions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        public class UpdateMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingKey_UpdatesEntity()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.IsActive = true;
                p2.PersonName = "Baz";

                personDal.Update(p2);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(true, p3.IsActive);
            }

            [Test]
            public void UsingKey_UpdatesPartProperties()
            {
                var personDal = new DalBase<PersonEntity>();

                PersonEntity p1 = new PersonEntity
                {
                    PersonName = "Bar",
                    CarId = 1,
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                personDal.Update(p2, new[] { "PersonName", "CarId", "CarName" });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                personDal.Update(new { p2.PersonId, p2.PersonName, p2.CarId, CarName = "CarName" });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                var predicate = Predicates.Field<PersonEntity>(f => f.PersonId, Operator.Eq, p2.PersonId);

                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                var predicate = Predicates.Field<PersonEntity>(f => f.PersonName, Operator.Eq, p1.PersonName);

                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    new { p2.PersonId });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    new { p1.PersonName });

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                Expression<Func<PersonEntity, bool>> predicate = p => p.PersonId == p2.PersonId;
                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                var p2 = personDal.Get(id);
                p2.PersonName = "Baz";
                p2.CarId = 2;
                p2.IsActive = true;

                Expression<Func<PersonEntity, bool>> predicate = p => p.PersonName == p1.PersonName;
                personDal.Update(new { p2.PersonName, p2.CarId, CarName = "CarName" },
                    predicate);

                var p3 = personDal.Get(id);
                Assert.AreEqual("Baz", p3.PersonName);
                Assert.AreEqual(2, p3.CarId);
                Assert.AreEqual(false, p3.IsActive);
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
                    IsActive = false
                };
                int id = personDal.Insert(p1);

                PersonEntity p2 = personDal.Get(id);
                Assert.AreEqual(id, p2.PersonId);
                Assert.AreEqual("Bar", p2.PersonName);
            }

        }

        [TestFixture]
        public class GetListMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNullPredicate_ReturnsAll()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.GetList();
                Assert.AreEqual(4, list.Count());
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsAllOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

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

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(2, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "a" || p.PersonName == "c"));
            }

            [Test]
            public void UsingNullPredicate_BySort_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };
                IEnumerable<PersonEntity> list = personDal.GetList((object)null, sort).ToList();

                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_BySort_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }


            [Test]
            public void UsingObject_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true, PersonName = "c" };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate);
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "c"));
            }

            [Test]
            public void UsingNullPredicate_ByObject_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList((object)null, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, SortDirection.Descending, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ReturnsMatching()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.GetList(p => p.IsActive == true && p.PersonName == "c");
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "c"));

                list = personDal.GetList(p => p.IsActive != true && p.PersonName == "b");
                Assert.AreEqual(1, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName == "b"));

                list = personDal.GetList(p => p.PersonName != "b");
                Assert.AreEqual(3, list.Count());
                Assert.IsTrue(list.All(p => p.PersonName != "b"));
            }

            [Test]
            public void UsingNullPredicate_ByExpression_ReturnsOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

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

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                var sort = new { CarId = SortDirection.Descending };
                IEnumerable<PersonEntity> list = personDal.GetList(predicate, sort).ToList();
                Assert.AreEqual(3, list.First().CarId);
                Assert.AreEqual(1, list.Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
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

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetListPaged(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, 2, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetListPaged(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, 2, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetListPaged(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetListPaged(predicate, sort, 2, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
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

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                var sort = new List<Sort>() { new Sort { PropertyName = "CarId", Ascending = false } };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingPredicate_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingObject_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByObject_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                var sort = new { CarId = SortDirection.Descending };

                Assert.AreEqual(4, personDal.GetSet(predicate, sort, 1, 2).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, sort, 3, 2).ToList().Last().CarId);
            }

            [Test]
            public void UsingExpression_ByExpression_ReturnsMatchingOrdered()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 4, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 2, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                Expression<Func<PersonEntity, object>> sort = p => p.CarId;

                Assert.AreEqual(4, personDal.GetSet(predicate, 1, 2, SortDirection.Descending, sort).ToList().First().CarId);
                Assert.AreEqual(1, personDal.GetSet(predicate, 3, 2, SortDirection.Descending, sort).ToList().Last().CarId);
            }

        }

        [TestFixture]
        public class CountMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingPredicate_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Assert.AreEqual(4, personDal.Count(null));
                var predicate = Predicates.Field<PersonEntity>(f => f.IsActive, Operator.Eq, true);
                Assert.AreEqual(2, personDal.Count(predicate));
            }

            [Test]
            public void UsingObject_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                var predicate = new { IsActive = true };
                Assert.AreEqual(2, personDal.Count(predicate));
            }

            [Test]
            public void UsingNullExpression_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = null;
                Assert.AreEqual(4, personDal.Count(predicate));
            }


            [Test]
            public void UsingExpression_Returns_Count()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                Expression<Func<PersonEntity, bool>> predicate = p => p.IsActive == true;
                Assert.AreEqual(2, personDal.Count(predicate));
            }

        }

        [TestFixture]
        public class QueryMethod : SqlServerBaseFixture
        {
            [Test]
            public void UsingNone_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "select * from Person where CarId = 3");

                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingParameter_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "select * from Person where CarId = @CarId", new { CarId = 3 });

                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingParameter_WithProcedure_ReturnsEntitys()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonEntity> list = personDal.Query(
                    "P_GetPersonsByCarId", new { CarId = 3 }, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
            }

            class PersonModel
            {
                public string Name { get; set; }

                public string CarId { get; set; }
            }

            [Test]
            public void UsingNone_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "select PersonName as Name, CarId from Person where CarId = 3");

                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingParameter_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "select PersonName as Name, CarId from Person where CarId = @CarId", new { CarId = 3 });

                Assert.AreEqual(2, list.Count());
            }

            [Test]
            public void UsingParameter_WithProcedure_ReturnsModels()
            {
                var personDal = new DalBase<PersonEntity>();

                personDal.Insert(new PersonEntity { PersonName = "a", CarId = 1, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "b", CarId = 3, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "c", CarId = 3, IsActive = true, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });
                personDal.Insert(new PersonEntity { PersonName = "d", CarId = 2, IsActive = false, CreateTime = DateTime.Now, UpdateTime = DateTime.Now });

                IEnumerable<PersonModel> list = personDal.Query<PersonModel>(
                    "P_GetPersonModelsByCarId",
                    new { CarId = 3 }, System.Data.CommandType.StoredProcedure);

                Assert.AreEqual(2, list.Count());
            }

        }
    }
}
