//// Copyright (C) 2009-2010 ORMBattle.NET.
//// All rights reserved.
//// For conditions of distribution and use, see license.
//// Created by: Alexis Kochetov
//// Created:    2009.07.31

//// This file is generated from LinqTests.tt

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using ExampleModel.Northwind;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Modl;

//namespace Tests
//{
//    [TestClass]
//    public class LinqSuite
//    {
//        protected NorthwindContext db;

//        //public override string ToolName {
//        //  get { return "ADO.NET Entity Framework"; }
//        //}

//        //public override string ShortToolName {
//        //  get { return "EF"; }
//        //}

//        //public override string SourceFileName {
//        //  get { return @"EFTest.generated.cs"; }
//        //}
//        //[TestInitialize]
//        public LinqSuite()
//        {
//            db = new NorthwindContext(Database.Get("Northwind"));

//            Customers = db.Customers.ToList();
//            Employees = db.Employees.ToList();
//            Orders = db.Orders.ToList();
//            Products = db.Products.ToList();
//        }

//        //protected override void TearDown()
//        //{
//        //  db.Dispose();
//        //}

//        List<Customer> Customers;
//        List<Employee> Employees;
//        List<Order> Orders;
//        List<Product> Products;

//        // DTO for testing purposes.
//        public class OrderDTO
//        {
//            public int Id { get; set; }
//            public string CustomerId { get; set; }
//            public DateTime? OrderDate { get; set; }
//        }

//        #region Filtering tests

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereTest()
//        {
//            var result = from o in db.Orders
//                         where o.ShipCity == "Seattle"
//                         select o;
//            var expected = from o in Orders
//                           where o.ShipCity == "Seattle"
//                           select o;
//            var list = result.ToList();
//            Assert.AreEqual(14, list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereParameterTest()
//        {
//            var city = "Seattle";
//            var result = from o in db.Orders
//                         where o.ShipCity == city
//                         select o;
//            var expected = from o in Orders
//                           where o.ShipCity == city
//                           select o;
//            var list = result.ToList();
//            Assert.AreEqual(14, list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());

//            city = "Rio de Janeiro";
//            list = result.ToList();
//            Assert.AreEqual(34, list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereConditionsTest()
//        {
//            var result = from p in db.Products
//                         where p.UnitsInStock < p.ReorderLevel && p.UnitsOnOrder == 0
//                         select p;
//            var list = result.ToList();
//            Assert.AreEqual(1, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereNullTest()
//        {
//            var result = from o in db.Orders
//                         where o.ShipRegion == null
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(507, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Failed with assertion.
//        // Exception: AssertionException
//        // Message:
//        //     Expected: 507
//        //     But was:  0
//        public void WhereNullParameterTest()
//        {
//            string region = null;
//            var result = from o in db.Orders
//                         where o.ShipRegion == region
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(507, list.Count);

//            region = "WA";
//            list = result.ToList();
//            Assert.AreEqual(19, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereNullableTest()
//        {
//            var result = from o in db.Orders
//                         where !o.ShippedDate.HasValue
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(21, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Failed with assertion.
//        // Exception: AssertionException
//        // Message:
//        //     Expected: 21
//        //     But was:  0
//        public void WhereNullableParameterTest()
//        {
//            DateTime? shippedDate = null;
//            var result = from o in db.Orders
//                         where o.ShippedDate == shippedDate
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(21, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereCoalesceTest()
//        {
//            var result = from o in db.Orders
//                         where (o.ShipRegion ?? "N/A") == "N/A"
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(507, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereConditionalTest()
//        {
//            var result = from o in db.Orders
//                         where (o.ShipCity == "Seattle" ? "Home" : "Other") == "Home"
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(14, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Passed.
//        public void WhereConditionalBooleanTest()
//        {
//            var result = from o in db.Orders
//                         where o.ShipCity == "Seattle" ? true : false
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(14, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   Unable to create a constant value of type 'Anonymous type'. Only primitive types ('such as Int32, String, and Guid') are supported in this context.
//        public void WhereAnonymousParameterTest()
//        {
//            var cityRegion = new { City = "Seattle", Region = "WA" };
//            var result = from o in db.Orders
//                         where new { City = o.ShipCity, Region = o.ShipRegion } == cityRegion
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(14, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Filtering")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   Unable to create a constant value of type 'OrmBattle.EFModel.Order'. Only primitive types ('such as Int32, String, and Guid') are supported in this context.
//        public void WhereEntityParameterTest()
//        {
//            var order = db.Orders.OrderBy(o => o.OrderDate).First();
//            var result = from o in db.Orders
//                         where o == order
//                         select o;
//            var list = result.ToList();
//            Assert.AreEqual(1, list.Count);
//            Assert.AreEqual(order, list[0]);
//            Assert.AreSame(order, list[0]);
//        }

//        #endregion

//        #region Projection tests

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectTest()
//        {
//            var result = from o in db.Orders
//                         select o.ShipRegion;
//            var expected = from o in Orders
//                           select o.ShipRegion;
//            var list = result.ToList();
//            Assert.AreEqual(expected.Count(), list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Failed.
//        // Exception: InvalidOperationException
//        // Message:
//        //   The cast to value type 'Boolean' failed because the materialized value is null. Either the result type's generic parameter or the query must use a nullable type.
//        public void SelectBooleanTest()
//        {
//            var result = from o in db.Orders
//                         select o.ShipRegion == "WA";
//            var expected = from o in Orders
//                           select o.ShipRegion == "WA";
//            var list = result.ToList();
//            Assert.AreEqual(expected.Count(), list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectCalculatedTest()
//        {
//            var result = from o in db.Orders
//                         select o.Freight * 1000;
//            var expected = from o in Orders
//                           select o.Freight * 1000;
//            var list = result.ToList();
//            var expectedList = expected.ToList();
//            list.Sort();
//            expectedList.Sort();

//            // Assert.AreEqual(expectedList.Count, list.Count);
//            // expectedList.Zip(list, (i, j) => {
//            //                       Assert.AreEqual(i,j);
//            //                       return true;
//            //                     });
//            CollectionAssert.AreEquivalent(expectedList, list);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectNestedCalculatedTest()
//        {
//            var result = from r in
//                             from o in db.Orders
//                             select o.Freight * 1000
//                         where r > 100000
//                         select r / 1000;
//            var expected = from o in Orders
//                           where o.Freight > 100
//                           select o.Freight;
//            var list = result.ToList();
//            var expectedList = expected.ToList();
//            list.Sort();
//            expectedList.Sort();
//            Assert.AreEqual(187, list.Count);
//            // Assert.AreEqual(expectedList.Count, list.Count);
//            // expectedList.Zip(list, (i, j) => {
//            //                       Assert.AreEqual(i,j);
//            //                       return true;
//            //                     });
//            CollectionAssert.AreEquivalent(expectedList, list);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectAnonymousTest()
//        {
//            var result = from o in db.Orders
//                         select new { OrderID = o.OrderId, o.OrderDate, o.Freight };
//            var expected = from o in Orders
//                           select new { OrderID = o.OrderId, o.OrderDate, o.Freight };
//            var list = result.ToList();
//            Assert.AreEqual(expected.Count(), list.Count);
//            Assert.AreEqual(0, expected.Except(list).Count());
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectSubqueryTest()
//        {
//            var result = from o in db.Orders
//                         select db.Customers.Where(c => c.CustomerId == o.Customer.CustomerId);
//            var expected = from o in Orders
//                           select Customers.Where(c => c.CustomerId == o.Customer.CustomerId);
//            var list = result.ToList();

//            var expectedList = expected.ToList();
//            CollectionAssert.AreEquivalent(expectedList, list);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectDtoTest()
//        {
//            var result = from o in db.Orders
//                         select new OrderDTO { Id = o.OrderId, CustomerId = o.Customer.CustomerId, OrderDate = o.OrderDate };
//            var list = result.ToList();
//            Assert.AreEqual(Orders.Count(), list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectNestedDtoTest()
//        {
//            var result = from r in
//                             from o in db.Orders
//                             select new OrderDTO { Id = o.OrderId, CustomerId = o.Customer.CustomerId, OrderDate = o.OrderDate }
//                         where r.OrderDate > new DateTime(1998, 01, 01)
//                         select r;
//            var list = result.ToList();
//            Assert.AreEqual(267, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectManyAnonymousTest()
//        {
//            var result = from c in db.Customers
//                         from o in c.Orders
//                         where o.Freight < 500.00M
//                         select new { CustomerId = c.CustomerId, o.OrderId, o.Freight };
//            var list = result.ToList();
//            Assert.AreEqual(817, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectManyLetTest()
//        {
//            var result = from c in db.Customers
//                         from o in c.Orders
//                         let freight = o.Freight
//                         where freight < 500.00M
//                         select new { CustomerId = c.CustomerId, o.OrderId, freight };
//            var list = result.ToList();
//            Assert.AreEqual(817, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectManyGroupByTest()
//        {
//            var result = db.Orders
//              .GroupBy(o => o.Customer)
//              .Where(g => g.Count() > 20)
//              .SelectMany(g => g.Select(o => o.Customer));

//            var list = result.ToList();
//            Assert.AreEqual(89, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectManyOuterProjectionTest()
//        {
//            var result = db.Customers.SelectMany(i => i.Orders.Select(t => i));

//            var list = result.ToList();
//            Assert.AreEqual(830, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Projections")]
//        // Passed.
//        public void SelectManyLeftJoinTest()
//        {
//            var result =
//              from c in db.Customers
//              from o in c.Orders.Select(o => new { o.OrderId, c.CompanyName }).DefaultIfEmpty()
//              select new { c.ContactName, o };

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        #endregion

//        #region Take / Skip tests

//        [TestMethod]
//        [TestCategory("Take/Skip")]
//        // Passed.
//        public void TakeTest()
//        {
//            var result = (from o in db.Orders
//                          orderby o.OrderDate, o.OrderId
//                          select o).Take(10);
//            var expected = (from o in Orders
//                            orderby o.OrderDate, o.OrderId
//                            select o).Take(10);
//            var list = result.ToList();
//            Assert.AreEqual(10, list.Count);
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Take/Skip")]
//        // Passed.
//        public void SkipTest()
//        {
//            var result = (from o in db.Orders
//                          orderby o.OrderDate, o.OrderId
//                          select o).Skip(10);
//            var expected = (from o in Orders
//                            orderby o.OrderDate, o.OrderId
//                            select o).Skip(10);
//            var list = result.ToList();
//            Assert.AreEqual(820, list.Count);
//            Assert.IsTrue(expected.SequenceEqual(result));

//        }

//        [TestMethod]
//        [TestCategory("Take/Skip")]
//        // Passed.
//        public void TakeSkipTest()
//        {
//            var result = (from o in db.Orders
//                          orderby o.OrderDate, o.OrderId
//                          select o).Skip(10).Take(10);
//            var expected = (from o in Orders
//                            orderby o.OrderDate, o.OrderId
//                            select o).Skip(10).Take(10);
//            var list = result.ToList();
//            Assert.AreEqual(10, list.Count);
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Take/Skip")]
//        // Passed.
//        public void TakeNestedTest()
//        {
//            var result =
//              from c in db.Customers
//              select new { Customer = c, TopOrders = c.Orders.OrderByDescending(o => o.OrderDate).Take(5) };
//            var expected =
//              from c in Customers
//              select new { Customer = c, TopOrders = c.Orders.OrderByDescending(o => o.OrderDate).Take(5) };
//            var list = result.ToList();
//            Assert.AreEqual(expected.Count(), list.Count);
//            foreach (var anonymous in list)
//            {
//                var count = anonymous.TopOrders.ToList().Count;
//                Assert.IsTrue(count >= 0);
//                Assert.IsTrue(count <= 5);
//            }
//        }

//        [TestMethod]
//        [TestCategory("Take/Skip")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   The method 'Skip' is only supported for sorted input in LINQ to Entities. The method 'OrderBy' must be called before the method 'Skip'.
//        public void ComplexTakeSkipTest()
//        {
//            var original = db.Orders.ToList()
//              .OrderBy(o => o.OrderDate)
//              .Skip(100)
//              .Take(50)
//              .OrderBy(o => o.RequiredDate)
//              .Where(o => o.OrderDate != null)
//              .Select(o => o.RequiredDate)
//              .Distinct()
//              .Skip(10);
//            var result = db.Orders
//              .OrderBy(o => o.OrderDate)
//              .Skip(100)
//              .Take(50)
//              .OrderBy(o => o.RequiredDate)
//              .Where(o => o.OrderDate != null)
//              .Select(o => o.RequiredDate)
//              .Distinct()
//              .Skip(10);
//            var originalList = original.ToList();
//            var resultList = result.ToList();
//            Assert.AreEqual(originalList.Count, resultList.Count);
//            Assert.IsTrue(originalList.SequenceEqual(resultList));
//        }

//        #endregion

//        #region Ordering tests

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByTest()
//        {
//            var result =
//              from o in db.Orders
//              orderby o.OrderDate, o.ShippedDate descending, o.OrderId
//              select o;
//            var expected =
//              from o in Orders
//              orderby o.OrderDate, o.ShippedDate descending, o.OrderId
//              select o;

//            var list = result.ToList();
//            var expectedList = expected.ToList();
//            Assert.AreEqual(expectedList.Count, list.Count);
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByWhereTest()
//        {
//            var result = (from o in db.Orders
//                          orderby o.OrderDate, o.OrderId
//                          where o.OrderDate > new DateTime(1997, 1, 1)
//                          select o).Take(10);
//            var expected = (from o in Orders
//                            where o.OrderDate > new DateTime(1997, 1, 1)
//                            orderby o.OrderDate, o.OrderId
//                            select o).Take(10);
//            var list = result.ToList();
//            Assert.AreEqual(10, list.Count);
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByCalculatedColumnTest()
//        {
//            var result =
//              from o in db.Orders
//              orderby o.Freight * o.OrderId descending
//              select o;
//            var expected =
//              from o in Orders
//              orderby o.Freight * o.OrderId descending
//              select o;
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Failed.
//        // Exception: ArgumentException
//        // Message:
//        //   DbSortClause expressions must have a type that is order comparable.
//        //   Parameter name: key
//        public void OrderByEntityTest()
//        {
//            var result =
//              from o in db.Orders
//              orderby o
//              select o;
//            var expected =
//              from o in Orders
//              orderby o.OrderId
//              select o;
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByAnonymousTest()
//        {
//            var result =
//              from o in db.Orders
//              orderby new { o.OrderDate, o.ShippedDate, o.OrderId }
//              select o;
//            var expected =
//              from o in Orders
//              orderby o.OrderDate, o.ShippedDate, o.OrderId
//              select o;
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByDistinctTest()
//        {
//            var result = db.Customers
//              .OrderBy(c => c.CompanyName)
//              .Select(c => c.City)
//              .Distinct()
//              .OrderBy(c => c)
//              .Select(c => c);
//            var expected = Customers
//              .OrderBy(c => c.CompanyName)
//              .Select(c => c.City)
//              .Distinct()
//              .OrderBy(c => c)
//              .Select(c => c);
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Failed with assertion.
//        // Exception: AssertionException
//        // Message:
//        //     Expected: True
//        //     But was:  False
//        public void OrderBySelectManyTest()
//        {
//            var result =
//              from c in db.Customers.OrderBy(c => c.ContactName)
//              from o in db.Orders.OrderBy(o => o.OrderDate)
//              where c == o.Customer
//              select new { c.ContactName, o.OrderDate };
//            var expected =
//              from c in Customers.OrderBy(c => c.ContactName)
//              from o in Orders.OrderBy(o => o.OrderDate)
//              where c == o.Customer
//              select new { c.ContactName, o.OrderDate };
//            Assert.IsTrue(expected.SequenceEqual(result));
//        }

//        [TestMethod]
//        [TestCategory("Ordering")]
//        // Passed.
//        public void OrderByPredicateTest()
//        {
//            var result = db.Orders.OrderBy(o => o.Freight > 0 && o.ShippedDate != null).ThenBy(o => o.OrderId).Select(o => o.OrderId);
//            var list = result.ToList();
//            var original = Orders.OrderBy(o => o.Freight > 0 && o.ShippedDate != null).ThenBy(o => o.OrderId).Select(o => o.OrderId).ToList();
//            Assert.IsTrue(list.SequenceEqual(original));
//        }

//        #endregion

//        #region Grouping tests

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByTest()
//        {
//            var result = from o in db.Orders
//                         group o by o.OrderDate;
//            var list = result.ToList();
//            Assert.AreEqual(480, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByReferenceTest()
//        {
//            var result = from o in db.Orders
//                         group o by o.Customer;
//            var list = result.ToList();
//            Assert.AreEqual(89, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByWhereTest()
//        {
//            var result =
//              from o in db.Orders
//              group o by o.OrderDate into g
//              where g.Count() > 5
//              select g;
//            var list = result.ToList();
//            Assert.AreEqual(1, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByTestAnonymous()
//        {
//            var result = from c in db.Customers
//                         group c by new { c.Region, c.City };
//            var list = result.ToList();
//            Assert.AreEqual(69, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByCalculatedTest()
//        {
//            var result =
//              from o in db.Orders
//              group o by o.Freight > 50 ? o.Freight > 100 ? "expensive" : "average" : "cheap" into g
//              select g;
//            var list = result.ToList();
//            Assert.AreEqual(3, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupBySelectManyTest()
//        {
//            var result = db.Customers
//              .GroupBy(c => c.City)
//              .SelectMany(g => g);

//            var list = result.ToList();
//            Assert.AreEqual(91, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByCalculateAggregateTest()
//        {
//            var result =
//              from o in db.Orders
//              group o by o.Customer into g
//              select g.Sum(o => o.Freight);

//            var list = result.ToList();
//            Assert.AreEqual(89, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void GroupByCalculateManyAggreagetes()
//        {
//            var result =
//              from o in db.Orders
//              group o by o.Customer into g
//              select new
//                      {
//                          Sum = g.Sum(o => o.Freight),
//                          Min = g.Min(o => o.Freight),
//                          Max = g.Max(o => o.Freight),
//                          Avg = g.Average(o => o.Freight)
//                      };

//            var list = result.ToList();
//            Assert.AreEqual(89, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Failed.
//        // Exception: InvalidOperationException
//        // Message:
//        //   The cast to value type 'Boolean' failed because the materialized value is null. Either the result type's generic parameter or the query must use a nullable type.
//        public void GroupByAggregate()
//        {
//            var result =
//              from c in db.Customers
//              group c by c.Orders.Average(o => o.Freight) >= 80;
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//            var firstGroupList = list.First(g => !g.Key).ToList();
//            Assert.AreEqual(71, firstGroupList.Count);
//        }

//        [TestMethod]
//        [TestCategory("Grouping")]
//        // Passed.
//        public void ComplexGroupingTest()
//        {
//            var result =
//              from c in db.Customers
//              select new
//              {
//                  c.CompanyName,
//                  YearGroups =
//                    from o in c.Orders
//                    group o by o.OrderDate.Value.Year into yg
//                    select new
//                    {
//                        Year = yg.Key,
//                        MonthGroups =
//                          from o in yg
//                          group o by o.OrderDate.Value.Month into mg
//                          select new { Month = mg.Key, Orders = mg }
//                    }
//              };
//            var list = result.ToList();
//            foreach (var customer in list)
//            {
//                var ordersList = customer.YearGroups.ToList();
//                Assert.IsTrue(ordersList.Count <= 3);
//            }
//        }

//        #endregion

//        #region Set operations / Distinct tests

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void ConcatTest()
//        {
//            var result = db.Customers.Where(c => c.Orders.Count <= 1).Concat(db.Customers.Where(c => c.Orders.Count > 1));
//            var list = result.ToList();
//            Assert.AreEqual(91, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void UnionTest()
//        {
//            var result = (
//                           from c in db.Customers
//                           select c.Phone)
//              .Union(
//              from c in db.Customers
//              select c.Fax)
//              .Union(
//              from e in db.Employees
//              select e.HomePhone
//              );

//            var list = result.ToList();
//            Assert.AreEqual(167, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void ExceptTest()
//        {
//            var result =
//              db.Customers.Except(db.Customers.Where(c => c.Orders.Count() > 0));
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void IntersectTest()
//        {
//            var result =
//              db.Customers.Intersect(db.Customers.Where(c => c.Orders.Count() > 0));
//            var list = result.ToList();
//            Assert.AreEqual(89, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void DistinctTest()
//        {
//            var result = db.Orders.Select(c => c.Freight).Distinct();
//            var list = result.ToList();
//            Assert.AreEqual(799, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void DistinctTakeLastTest()
//        {
//            var result =
//              (from o in db.Orders
//               orderby o.OrderDate
//               select o.OrderDate).Distinct().Take(5);
//            var list = result.ToList();
//            Assert.AreEqual(5, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void DistinctTakeFirstTest()
//        {
//            var result =
//              (from o in db.Orders
//               orderby o.OrderDate
//               select o.OrderDate).Take(5).Distinct();
//            var list = result.ToList();
//            Assert.AreEqual(4, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void DistinctEntityTest()
//        {
//            var result = db.Customers.Distinct();
//            var list = result.ToList();
//            Assert.AreEqual(91, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Set operations")]
//        // Passed.
//        public void DistinctAnonymousTest()
//        {
//            var result = db.Customers.Select(c => new { c.Region, c.City }).Distinct();
//            var list = result.ToList();
//            Assert.AreEqual(69, list.Count);
//        }

//        #endregion

//        #region Type casts

//        //[TestMethod]
//        //[TestCategory("Type casts")]
//        //// Passed.
//        //public void TypeCastIsChildTest()
//        //{
//        //    var result = db.Products.Where(p => p is DiscontinuedProduct);
//        //    var expected = db.Products.ToList().Where(p => p is DiscontinuedProduct);
//        //    var list = result.ToList();
//        //    Assert.IsTrue(list.Count > 0);
//        //    Assert.AreEqual(expected.Count(), list.Count);
//        //}

//        [TestMethod]
//        [TestCategory("Type casts")]
//        // Passed.
//        public void TypeCastIsParentTest()
//        {
//            var result = db.Products.Where(p => p is Product);
//            var expected = db.Products.ToList();
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//            Assert.AreEqual(expected.Count(), list.Count);
//        }

//        //[TestMethod]
//        //[TestCategory("Type casts")]
//        //// Passed.
//        //public void TypeCastIsChildConditionalTest()
//        //{
//        //    var result = db.Products
//        //      .Select(x => x is DiscontinuedProduct
//        //                     ? x
//        //                     : null);

//        //    var expected = db.Products.ToList()
//        //      .Select(x => x is DiscontinuedProduct
//        //                     ? x
//        //                     : null);

//        //    var list = result.ToList();
//        //    Assert.IsTrue(list.Count > 0);
//        //    Assert.AreEqual(expected.Count(), list.Count);
//        //    Assert.IsTrue(list.Except(expected).Count() == 0);
//        //    Assert.IsTrue(list.Contains(null));
//        //}

//        [TestMethod]
//        [TestCategory("Type casts")]
//        // Passed.
//        public void TypeCastOfTypeTest()
//        {
//            var result = db.Products.OfType<DiscontinuedProduct>();
//            var expected = db.Products.ToList().OfType<DiscontinuedProduct>();
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//            Assert.AreEqual(expected.Count(), list.Count);
//        }

//        //[TestMethod]
//        //[TestCategory("Type casts")]
//        //// Failed.
//        //// Exception: InvalidOperationException
//        //// Message:
//        ////   The specified cast from a materialized 'OrmBattle.EFModel.ActiveProduct' type to the 'OrmBattle.EFModel.DiscontinuedProduct' type is not valid.
//        //public void TypeCastAsTest()
//        //{
//        //    var result = db.DiscontinuedProducts
//        //      .Select(discontinuedProduct => discontinuedProduct as Product)
//        //      .Select(product =>
//        //              product == null
//        //                ? "NULL"
//        //                : product.ProductName);

//        //    var expected = db.DiscontinuedProducts.ToList()
//        //      .Select(discontinuedProduct => discontinuedProduct as Product)
//        //      .Select(product =>
//        //              product == null
//        //                ? "NULL"
//        //                : product.ProductName);

//        //    var list = result.ToList();
//        //    Assert.Greater(list.Count, 0);
//        //    Assert.AreEqual(expected.Count(), list.Count);
//        //    Assert.IsTrue(list.Except(expected).Count() == 0);
//        //}

//        #endregion

//        #region Element operations

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Passed.
//        public void FirstTest()
//        {
//            var customer = db.Customers.First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Passed.
//        public void FirstOrDefaultTest()
//        {
//            var customer = db.Customers.Where(c => c.CustomerId == "ALFKI").FirstOrDefault();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Passed.
//        public void FirstPredicateTest()
//        {
//            var customer = db.Customers.First(c => c.CustomerId == "ALFKI");
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Passed.
//        public void NestedFirstOrDefaultTest()
//        {
//            var result =
//              from p in db.Products
//              select new
//              {
//                  ProductID = p.ProductId,
//                  MaxOrder = db.OrderDetails
//                    .Where(od => od.Product == p)
//                    .OrderByDescending(od => od.UnitPrice * od.Quantity)
//                    .FirstOrDefault()
//                    .Order
//              };
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Passed.
//        public void FirstOrDefaultEntitySetTest()
//        {
//            var customersCount = Customers.Count;
//            var result = db.Customers.Select(c => c.Orders.FirstOrDefault());
//            var list = result.ToList();
//            Assert.AreEqual(customersCount, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   The methods 'Single' and 'SingleOrDefault' can only be used as a final query operation. Consider using the method 'FirstOrDefault' in this instance instead.
//        public void NestedSingleOrDefaultTest()
//        {
//            var customersCount = Customers.Count;
//            var result = db.Customers.Select(c => c.Orders.Take(1).SingleOrDefault());
//            var list = result.ToList();
//            Assert.AreEqual(customersCount, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   The methods 'Single' and 'SingleOrDefault' can only be used as a final query operation. Consider using the method 'FirstOrDefault' in this instance instead.
//        public void NestedSingleTest()
//        {
//            var result = db.Customers.Where(c => c.Orders.Count() > 0).Select(c => c.Orders.Take(1).Single());
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'OrmBattle.EFModel.Customer ElementAt[Customer](System.Linq.IQueryable`1[OrmBattle.EFModel.Customer], Int32)' method, and this method cannot be translated into a store expression.
//        public void ElementAtTest()
//        {
//            var customer = db.Customers.OrderBy(c => c.CustomerId).ElementAt(15);
//            Assert.IsNotNull(customer);
//            Assert.AreEqual("CONSH", customer.CustomerId);
//        }

//        [TestMethod]
//        [TestCategory("Element operations")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'OrmBattle.EFModel.Order ElementAt[Order](System.Collections.Generic.IEnumerable`1[OrmBattle.EFModel.Order], Int32)' method, and this method cannot be translated into a store expression.
//        public void NestedElementAtTest()
//        {
//            var result =
//              from c in db.Customers
//              where c.Orders.Count() > 5
//              select c.Orders.ElementAt(3);

//            var list = result.ToList();
//            Assert.AreEqual(63, list.Count);
//        }

//        #endregion


//        #region Contains / Any / All tests

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void AllNestedTest()
//        {
//            var result =
//              from c in db.Customers
//              where db.Orders.Where(o => o.Customer == c).All(o => db.Employees.Where(e => o.Employee == e).Any(e => e.FirstName.StartsWith("A")))
//              select c;
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void ComplexAllTest()
//        {
//            var result =
//              from o in db.Orders
//              where
//                db.Customers.Where(c => c == o.Customer).All(c => c.CompanyName.StartsWith("A")) ||
//                db.Employees.Where(e => e == o.Employee).All(e => e.FirstName.EndsWith("t"))
//              select o;
//            var expected =
//              from o in Orders
//              where
//                Customers.Where(c => c == o.Customer).All(c => c.CompanyName.StartsWith("A")) ||
//                Employees.Where(e => e == o.Employee).All(e => e.FirstName.EndsWith("t"))
//              select o;

//            Assert.AreEqual(0, expected.Except(result).Count());
//            Assert.AreEqual(result.ToList().Count, 366);
//        }

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void ContainsNestedTest()
//        {
//            var result = from c in db.Customers
//                         select new
//                                {
//                                    Customer = c,
//                                    HasNewOrders = db.Orders
//                             .Where(o => o.OrderDate > new DateTime(2001, 1, 1))
//                             .Select(o => o.Customer)
//                             .Contains(c)
//                                };

//            var expected =
//              from c in Customers
//              select new
//                     {
//                         Customer = c,
//                         HasNewOrders = Orders
//                  .Where(o => o.OrderDate > new DateTime(2001, 1, 1))
//                  .Select(o => o.Customer)
//                  .Contains(c)
//                     };
//            Assert.AreEqual(0, expected.Except(result).Count());
//            Assert.AreEqual(0, result.ToList().Count(i => i.HasNewOrders));
//        }

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void AnyTest()
//        {
//            var result = db.Customers.Where(c => c.Orders.Any(o => o.Freight > 400));
//            var expected = Customers.Where(c => c.Orders.Any(o => o.Freight > 400));
//            Assert.AreEqual(0, expected.Except(result).Count());
//            Assert.AreEqual(10, result.ToList().Count);
//        }

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void AnyParameterizedTest()
//        {
//            var ids = new[] { "ABCDE", "ALFKI" };
//            var result = db.Customers.Where(c => ids.Any(id => c.CustomerId == id));
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        [TestMethod]
//        [TestCategory("All/Any/Contains")]
//        // Passed.
//        public void ContainsParameterizedTest()
//        {
//            var customerIDs = new[] { "ALFKI", "ANATR", "AROUT", "BERGS" };
//            var result = db.Orders.Where(o => customerIDs.Contains(o.Customer.CustomerId));
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//            Assert.AreEqual(41, list.Count);
//        }

//        #endregion

//        #region Aggregates tests

//        [TestMethod]
//        [TestCategory("Aggregates")]
//        // Passed.
//        public void SumTest()
//        {
//            var sum = db.Orders.Select(o => o.Freight).Sum();
//            var sum1 = Orders.Select(o => o.Freight).Sum();
//            Assert.AreEqual(sum1, sum);
//        }

//        [TestMethod]
//        [TestCategory("Aggregates")]
//        // Passed.
//        public void CountPredicateTest()
//        {
//            var count = db.Orders.Count(o => o.OrderId > 10);
//            var count1 = Orders.Count(o => o.OrderId > 10);
//            Assert.AreEqual(count1, count);
//        }

//        [TestMethod]
//        [TestCategory("Aggregates")]
//        // Passed.
//        public void NestedCountTest()
//        {
//            var result = db.Customers.Where(c => db.Orders.Count(o => o.Customer.CustomerId == c.CustomerId) > 5);
//            var expected = Customers.Where(c => db.Orders.Count(o => o.Customer.CustomerId == c.CustomerId) > 5);

//            Assert.IsTrue(expected.Except(result).Count() == 0);
//        }

//        [TestMethod]
//        [TestCategory("Aggregates")]
//        // Passed.
//        public void NullableSumTest()
//        {
//            var sum = db.Orders.Select(o => (int?)o.OrderId).Sum();
//            var sum1 = Orders.Select(o => (int?)o.OrderId).Sum();
//            Assert.AreEqual(sum1, sum);
//        }

//        [TestMethod]
//        [TestCategory("Aggregates")]
//        // Passed.
//        public void MaxCountTest()
//        {
//            var max = db.Customers.Max(c => db.Orders.Count(o => o.Customer.CustomerId == c.CustomerId));
//            var max1 = Customers.Max(c => db.Orders.Count(o => o.Customer.CustomerId == c.CustomerId));
//            Assert.AreEqual(max1, max);
//        }

//        #endregion

//        #region Join tests

//        [TestMethod]
//        [TestCategory("Join")]
//        // Passed.
//        public void GroupJoinTest()
//        {
//            var result =
//              from c in db.Customers
//              join o in db.Orders on c.CustomerId equals o.Customer.CustomerId into go
//              join e in db.Employees on c.City equals e.City into ge
//              select new
//              {
//                  OrdersCount = go.Count(),
//                  EmployeesCount = ge.Count()
//              };
//            var list = result.ToList();
//            Assert.AreEqual(91, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Join")]
//        // Passed.
//        public void JoinTest()
//        {
//            var result =
//              from p in db.Products
//              join s in db.Suppliers on p.Supplier.SupplierId equals s.SupplierId
//              select new { p.ProductName, s.ContactName, s.Phone };

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        [TestMethod]
//        [TestCategory("Join")]
//        // Passed.
//        public void JoinByAnonymousTest()
//        {
//            var result =
//              from c in db.Customers
//              join o in db.Orders on new { Customer = c, Name = c.ContactName } equals
//                new { o.Customer, Name = o.Customer.ContactName }
//              select new { c.ContactName, o.OrderDate };

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        [TestMethod]
//        [TestCategory("Join")]
//        // Passed.
//        public void LeftJoinTest()
//        {
//            var result =
//              from c in db.Categories
//              join p in db.Products on c.CategoryId equals p.Category.CategoryId into g
//              from p in g.DefaultIfEmpty()
//              select new { Name = p == null ? "Nothing!" : p.ProductName, c.CategoryName };

//            var list = result.ToList();
//            Assert.AreEqual(77, list.Count);
//        }

//        #endregion

//        #region References tests

//        [TestMethod]
//        [TestCategory("References")]
//        // Passed.
//        public void JoinByReferenceTest()
//        {
//            var result =
//              from c in db.Customers
//              join o in db.Orders on c equals o.Customer
//              select new { c.ContactName, o.OrderDate };

//            var list = result.ToList();
//            Assert.AreEqual(830, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("References")]
//        // Passed.
//        public void CompareReferenceTest()
//        {
//            var result =
//              from c in db.Customers
//              from o in db.Orders
//              where c == o.Customer
//              select new { c.ContactName, o.OrderDate };

//            var list = result.ToList();
//            Assert.AreEqual(830, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("References")]
//        // Passed.
//        public void ReferenceNavigationTestTest()
//        {
//            var result =
//              from od in db.OrderDetails
//              where od.Product.Category.CategoryName == "Seafood"
//              select new { od.Order, od.Product };

//            var list = result.ToList();
//            Assert.AreEqual(330, list.Count);
//            foreach (var anonymous in list)
//            {
//                Assert.IsNotNull(anonymous);
//                Assert.IsNotNull(anonymous.Order);
//                Assert.IsNotNull(anonymous.Product);
//            }
//        }

//        [TestMethod]
//        [TestCategory("References")]
//        // Passed.
//        public void EntitySetCountTest()
//        {
//            var result = db.Categories.Where(c => c.Products.Count > 10);
//            var list = result.ToList();
//            Assert.AreEqual(4, list.Count);
//        }

//        #endregion


//        #region Complex tests

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest1()
//        {
//            var result = db.Suppliers.Select(
//              supplier => db.Products.Select(
//                            product => db.Products.Where(p => p.ProductId == product.ProductId && p.Supplier.SupplierId == supplier.SupplierId)));
//            var count = result.ToList().Count;
//            Assert.IsTrue(count > 0);
//            foreach (var queryable in result)
//            {
//                foreach (var queryable1 in queryable)
//                {
//                    foreach (var product in queryable1)
//                    {
//                        Assert.IsNotNull(product);
//                    }
//                }
//            }
//        }

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest2()
//        {
//            var result = db.Customers
//              .GroupBy(c => c.Country, (country, customers) => customers.Where(k => k.CompanyName.Substring(0, 1) == country.Substring(0, 1)))
//              .SelectMany(k => k);
//            var expected = Customers
//              .GroupBy(c => c.Country, (country, customers) => customers.Where(k => k.CompanyName.Substring(0, 1) == country.Substring(0, 1)))
//              .SelectMany(k => k);

//            Assert.AreEqual(0, expected.Except(result).Count());
//        }

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest3()
//        {
//            var products = db.Products;
//            var suppliers = db.Suppliers;
//            var result = from p in products
//                         select new
//                                {
//                                    Product = p,
//                                    Suppliers = suppliers
//                             .Where(s => s.SupplierId == p.Supplier.SupplierId)
//                             .Select(s => s.CompanyName)
//                                };
//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//            foreach (var p in list)
//                foreach (var companyName in p.Suppliers)
//                    Assert.IsNotNull(companyName);
//        }

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest4()
//        {
//            var result = db.Customers
//              .Take(2)
//              .Select(c => db.Orders.Select(o => db.Employees.Take(2).Where(e => e.Orders.Contains(o))).Where(o => o.Count() > 0))
//              .Select(os => os);

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);

//            foreach (var item in list)
//                item.ToList();
//        }

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest5()
//        {
//            var result = db.Customers
//              .Select(c => new { Customer = c, Orders = db.Orders })
//              .Select(i => i.Customer.Orders);

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);

//            foreach (var item in list)
//                item.ToList();
//        }

//        [TestMethod]
//        [TestCategory("Complex")]
//        // Passed.
//        public void ComplexTest6()
//        {
//            var result = db.Customers
//              .Select(c => new { Customer = c, Orders = db.Orders.Where(o => o.Customer == c) })
//              .SelectMany(i => i.Orders.Select(o => new { i.Customer, Order = o }));

//            var list = result.ToList();
//            Assert.IsTrue(list.Count > 0);
//        }

//        #endregion

//        #region Standard functions tests

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringStartsWithTest()
//        {
//            var result = db.Customers.Where(c => c.CustomerId.StartsWith("A") || c.CustomerId.StartsWith("L"));

//            var list = result.ToList();
//            Assert.AreEqual(13, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringStartsWithParameterizedTest()
//        {
//            var likeA = "A";
//            var likeL = "L";
//            var result = db.Customers.Where(c => c.CustomerId.StartsWith(likeA) || c.CustomerId.StartsWith(likeL));

//            var list = result.ToList();
//            Assert.AreEqual(13, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringLengthTest()
//        {
//            var customer = db.Customers.Where(c => c.City.Length == 7).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringContainsTest()
//        {
//            var customer = db.Customers.Where(c => c.ContactName.Contains("and")).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringToLowerTest()
//        {
//            var customer = db.Customers.Where(c => c.City.ToLower() == "seattle").First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringRemoveTest()
//        {
//            var customer = db.Customers.Where(c => c.City.Remove(3) == "Sea").First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringIndexOfTest()
//        {
//            var customer = db.Customers.Where(c => c.City.IndexOf("tt") == 3).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'Int32 LastIndexOf(System.String, Int32, Int32)' method, and this method cannot be translated into a store expression.
//        public void StringLastIndexOfTest()
//        {
//            var customer = db.Customers.Where(c => c.City.LastIndexOf("t", 1, 3) == 3).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'System.String PadLeft(Int32)' method, and this method cannot be translated into a store expression.
//        public void StringPadLeftTest()
//        {
//            var customer = db.Customers.Where(c => "123" + c.City.PadLeft(8) == "123 Seattle").First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   Only parameterless constructors and initializers are supported in LINQ to Entities.
//        public void DateTimeTest()
//        {
//            var order = db.Orders.Where(o => o.OrderDate >= new DateTime(o.OrderDate.Value.Year, 1, 1)).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void DateTimeDayTest()
//        {
//            var order = db.Orders.Where(o => o.OrderDate.Value.Day == 5).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   The specified type member 'DayOfWeek' is not supported in LINQ to Entities. Only initializers, entity members, and entity navigation properties are supported.
//        public void DateTimeDayOfWeek()
//        {
//            var order = db.Orders.Where(o => o.OrderDate.Value.DayOfWeek == DayOfWeek.Friday).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   The specified type member 'DayOfYear' is not supported in LINQ to Entities. Only initializers, entity members, and entity navigation properties are supported.
//        public void DateTimeDayOfYear()
//        {
//            var order = db.Orders.Where(o => o.OrderDate.Value.DayOfYear == 360).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void MathAbsTest()
//        {
//            var order = db.Orders.Where(o => Math.Abs(o.OrderId) == 10 || o.OrderId > 0).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'Double Asin(Double)' method, and this method cannot be translated into a store expression.
//        public void MathTrignometricTest()
//        {
//            var order = db.Orders.Where(o => Math.Asin(Math.Cos(o.OrderId)) == 0 || o.OrderId > 0).First();
//            Assert.IsNotNull(order);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void MathFloorTest()
//        {
//            var result = db.Orders.Where(o => Math.Floor(o.Freight) == 140);
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void MathCeilingTest()
//        {
//            var result = db.Orders.Where(o => Math.Ceiling(o.Freight) == 141);
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'System.Decimal Truncate(System.Decimal)' method, and this method cannot be translated into a store expression.
//        public void MathTruncateTest()
//        {
//            var result = db.Orders.Where(o => Math.Truncate(o.Freight) == 141);
//            var list = result.ToList();
//            Assert.AreEqual(2, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'System.Decimal Round(System.Decimal, Int32, System.MidpointRounding)' method, and this method cannot be translated into a store expression.
//        public void MathRoundAwayFromZeroTest()
//        {
//            var result = db.Orders.Where(o => Math.Round(o.Freight / 10, 1, MidpointRounding.AwayFromZero) == 6.5m);
//            var list = result.ToList();
//            Assert.AreEqual(7, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'System.Decimal Round(System.Decimal, Int32, System.MidpointRounding)' method, and this method cannot be translated into a store expression.
//        public void MathRoundToEvenTest()
//        {
//            var result = db.Orders.Where(o => Math.Round(o.Freight / 10, 1, MidpointRounding.ToEven) == 6.5m);
//            var list = result.ToList();
//            Assert.AreEqual(6, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed with assertion.
//        // Exception: AssertionException
//        // Message:
//        //     Expected: 6
//        //     But was:  7
//        public void MathRoundDefaultTest()
//        {
//            var result = db.Orders.Where(o => Math.Round(o.Freight / 10, 1) == 6.5m);
//            var list = result.ToList();
//            Assert.AreEqual(6, list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Failed.
//        // Exception: NotSupportedException
//        // Message:
//        //   LINQ to Entities does not recognize the method 'Int32 ToInt32(System.Decimal)' method, and this method cannot be translated into a store expression.
//        public void ConvertToInt32()
//        {
//            var expected = Orders.Where(o => Convert.ToInt32(o.Freight * 10) == 592);
//            var result = db.Orders.Where(o => Convert.ToInt32(o.Freight * 10) == 592);
//            var list = result.ToList();
//            Assert.AreEqual(expected.Count(), list.Count);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void StringCompareToTest()
//        {
//            var customer = db.Customers.Where(c => c.City.CompareTo("Seattle") >= 0).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void ComparisonWithNullTest()
//        {
//            var customer = db.Customers.Where(c => null != c.City).First();
//            Assert.IsNotNull(customer);
//        }

//        [TestMethod]
//        [TestCategory("Standard functions")]
//        // Passed.
//        public void EqualsWithNullTest()
//        {
//            var customer = db.Customers.Where(c => !c.Address.Equals(null)).First();
//            Assert.IsNotNull(customer);
//        }

//        #endregion
//    }
//}
