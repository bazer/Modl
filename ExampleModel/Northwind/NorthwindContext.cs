using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;

namespace ExampleModel.Northwind
{
    public class NorthwindContext
    {
        Database db;

        public NorthwindContext(Database db)
        {
            this.db = db;
        }

        public IQueryable<Category> Categories
        {
            get { return DbModl<Category>.Query(db); }
        }

        public IQueryable<Customer> Customers
        {
            get { return DbModl < Customer>.Query(db); ; }
        }

        public IQueryable<Employee> Employees
        {
            get { return DbModl<Employee>.Query(db); }
        }

        public IQueryable<Order> Orders
        {
            get { return DbModl<Order>.Query(db); }
        }

        public IQueryable<OrderDetail> OrderDetails
        {
            get { return DbModl<OrderDetail>.Query(db); }
        }

        public IQueryable<Product> Products
        {
            get { return DbModl<Product>.Query(db); }
        }

        public IQueryable<ActiveProduct> ActiveProducts
        {
            get { return DbModl<ActiveProduct>.Query(db); }
        }

        public IQueryable<DiscontinuedProduct> DiscontinuedProducts
        {
            get { return DbModl<DiscontinuedProduct>.Query(db); }
        }

        public IQueryable<Region> Regions
        {
            get { return DbModl<Region>.Query(db); }
        }

        public IQueryable<Shipper> Shippers
        {
            get { return DbModl<Shipper>.Query(db); }
        }

        public IQueryable<Supplier> Suppliers
        {
            get { return DbModl<Supplier>.Query(db); }
        }

        public IQueryable<Territory> Territories
        {
            get { return DbModl<Territory>.Query(db); }
        }
    }
}
