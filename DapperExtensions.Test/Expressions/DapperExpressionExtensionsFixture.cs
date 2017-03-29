using DapperExtensions.Expressions;
using DapperExtensions.Test.Data;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DapperExtensions.Test.Expressions
{
    [TestFixture]
    public class DapperExpressionExtensionsFixture
    {
        [TestFixture]
        public class OneExpression
        {
            [Test]
            public void EqOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.FirstName == "Foo";
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.Active == true;
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.FirstName.Equals("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void NotEqOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id != 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();

                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1, not: true);

                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.FirstName != "Foo";
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo", not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.Active != true;
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Active, Operator.Eq, true, not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !p.FirstName.Equals("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo", not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void GtOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id > 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Gt, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void GeOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id >= 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Ge, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void LtOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id < 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Lt, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void LeOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id <= 1;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Le, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void LikeOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.FirstName.Contains("Foo");
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "%Foo%");
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.FirstName.StartsWith("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "Foo%");
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => p.FirstName.EndsWith("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "%Foo");
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void NotLikeOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => !p.FirstName.Contains("Foo");
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "%Foo%", not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !p.FirstName.StartsWith("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "Foo%", not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !p.FirstName.EndsWith("Foo");
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.FirstName, Operator.Like, "%Foo", not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }
        }

        [TestFixture]
        public class SpecialExpression
        {
            [Test]
            public void NotOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => !(p.Id == 1);
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1, not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !(p.Id == (int)1L);
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1, not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void ConvertOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == (int)1L;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => (long)p.Id == 1L;
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void BoolOperator_ReturnsFieldPredicate()
            {
                Expression<Func<Person, bool>> expression = p => p.Active;
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !p.Active;
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Active, Operator.Eq, true, not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

            [Test]
            public void InOperator_ReturnsFieldPredicate()
            {
                var list = new[] { 1, 2, 3 };
                Expression<Func<Person, bool>> expression = p => list.Contains(p.Id);
                IFieldPredicate actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, new[] { 1, 2, 3 });
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);

                expression = p => !list.Contains(p.Id);
                actual = (IFieldPredicate)expression.ToPredicateGroup<Person, int>();
                expected = Predicates.Field<Person>(p => p.Id, Operator.Eq, new[] { 1, 2, 3 }, not: true);
                Assert.AreEqual(expected.PropertyName, actual.PropertyName);
                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(expected.Not, actual.Not);
                Assert.AreEqual(expected.Value, actual.Value);
            }

        }

        [TestFixture]
        public class TwoExpression_AndJoin
        {
            [Test]
            public void EqOperator_ReturnsPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 && p.Active == true;
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IPredicateGroup expected = Predicates.Group(GroupOperator.And, first, second);

                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(2, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates.Count, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates[0].GetType(), actual.Predicates[0].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).PropertyName, ((IFieldPredicate)actual.Predicates[0]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Operator, ((IFieldPredicate)actual.Predicates[0]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Not, ((IFieldPredicate)actual.Predicates[0]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Value, ((IFieldPredicate)actual.Predicates[0]).Value);
                Assert.AreEqual(expected.Predicates[1].GetType(), actual.Predicates[1].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).PropertyName, ((IFieldPredicate)actual.Predicates[1]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Operator, ((IFieldPredicate)actual.Predicates[1]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Not, ((IFieldPredicate)actual.Predicates[1]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Value, ((IFieldPredicate)actual.Predicates[1]).Value);
            }

            [Test]
            public void NotEqOperator_ReturnsPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id != 1 && p.Active != true;
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1, not: true);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true, not: true);
                IPredicateGroup expected = Predicates.Group(GroupOperator.And, first, second);

                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(2, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates.Count, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates[0].GetType(), actual.Predicates[0].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).PropertyName, ((IFieldPredicate)actual.Predicates[0]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Operator, ((IFieldPredicate)actual.Predicates[0]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Not, ((IFieldPredicate)actual.Predicates[0]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Value, ((IFieldPredicate)actual.Predicates[0]).Value);
                Assert.AreEqual(expected.Predicates[1].GetType(), actual.Predicates[1].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).PropertyName, ((IFieldPredicate)actual.Predicates[1]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Operator, ((IFieldPredicate)actual.Predicates[1]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Not, ((IFieldPredicate)actual.Predicates[1]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Value, ((IFieldPredicate)actual.Predicates[1]).Value);
            }

        }

        [TestFixture]
        public class TwoExpression_OrJoin
        {
            [Test]
            public void EqOperator_ReturnsPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 || p.Active == true;
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IPredicateGroup expected = Predicates.Group(GroupOperator.Or, first, second);

                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(2, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates.Count, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates[0].GetType(), actual.Predicates[0].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).PropertyName, ((IFieldPredicate)actual.Predicates[0]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Operator, ((IFieldPredicate)actual.Predicates[0]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Not, ((IFieldPredicate)actual.Predicates[0]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Value, ((IFieldPredicate)actual.Predicates[0]).Value);
                Assert.AreEqual(expected.Predicates[1].GetType(), actual.Predicates[1].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).PropertyName, ((IFieldPredicate)actual.Predicates[1]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Operator, ((IFieldPredicate)actual.Predicates[1]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Not, ((IFieldPredicate)actual.Predicates[1]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Value, ((IFieldPredicate)actual.Predicates[1]).Value);
            }

            [Test]
            public void NotEqOperator_ReturnsPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id != 1 || p.Active != true;
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1, not: true);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true, not: true);
                IPredicateGroup expected = Predicates.Group(GroupOperator.Or, first, second);

                Assert.AreEqual(expected.Operator, actual.Operator);
                Assert.AreEqual(2, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates.Count, actual.Predicates.Count);
                Assert.AreEqual(expected.Predicates[0].GetType(), actual.Predicates[0].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).PropertyName, ((IFieldPredicate)actual.Predicates[0]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Operator, ((IFieldPredicate)actual.Predicates[0]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Not, ((IFieldPredicate)actual.Predicates[0]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[0]).Value, ((IFieldPredicate)actual.Predicates[0]).Value);
                Assert.AreEqual(expected.Predicates[1].GetType(), actual.Predicates[1].GetType());
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).PropertyName, ((IFieldPredicate)actual.Predicates[1]).PropertyName);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Operator, ((IFieldPredicate)actual.Predicates[1]).Operator);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Not, ((IFieldPredicate)actual.Predicates[1]).Not);
                Assert.AreEqual(((IFieldPredicate)expected.Predicates[1]).Value, ((IFieldPredicate)actual.Predicates[1]).Value);
            }
        }

        [TestFixture]
        public class ThreeExpression_AndAndJoin
        {
            [Test]
            public void EqOperator_ReturnsOrPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 && p.Active == true && p.FirstName == "Foo";
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.And, first, second, third);
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }
        }

        [TestFixture]
        public class ThreeExpression_OrOrJoin
        {
            [Test]
            public void EqOperator_ReturnsOrPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 || p.Active == true || p.FirstName == "Foo";
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.Or, first, second, third);
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }
        }

        [TestFixture]
        public class ThreeExpression_AndOrJoin
        {
            [Test]
            public void EqOperator_ReturnsOrPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 && p.Active == true || p.FirstName == "Foo";
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.Or, Predicates.Group(GroupOperator.And, first, second), third);
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }

            [Test]
            public void EqOperator_ReturnsAndPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 && (p.Active == true || p.FirstName == "Foo");
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.And, first, Predicates.Group(GroupOperator.Or, second, third));
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }
        }

        [TestFixture]
        public class ThreeExpression_OrAndJoin
        {

            [Test]
            public void EqOperator_ReturnsOrPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => p.Id == 1 || p.Active == true && p.FirstName == "Foo";
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.Or, first, Predicates.Group(GroupOperator.And, second, third));
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }

            [Test]
            public void EqOperator_ReturnsAndPredicateGroup()
            {
                Expression<Func<Person, bool>> expression = p => (p.Id == 1 || p.Active == true) && p.FirstName == "Foo";
                IPredicateGroup actual = (IPredicateGroup)expression.ToPredicateGroup<Person, int>();
                var actualJson = JsonConvert.SerializeObject(actual, formatting: Formatting.Indented);

                IFieldPredicate first = Predicates.Field<Person>(p => p.Id, Operator.Eq, 1);
                IFieldPredicate second = Predicates.Field<Person>(p => p.Active, Operator.Eq, true);
                IFieldPredicate third = Predicates.Field<Person>(p => p.FirstName, Operator.Eq, "Foo");
                IPredicateGroup expected = Predicates.Group(GroupOperator.And, Predicates.Group(GroupOperator.Or, first, second), third);
                var expectedJson = JsonConvert.SerializeObject(expected, formatting: Formatting.Indented);

                Assert.AreEqual(expectedJson, actualJson);
            }
        }

    }
}
