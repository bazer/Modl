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
            get { return db.Query<Category>(); }
        }

        public IQueryable<Customer> Customers
        {
            get { return db.Query<Customer>(); }
        }

        public IQueryable<Employee> Employees
        {
            get { return db.Query<Employee>(); }
        }

        public IQueryable<Order> Orders
        {
            get { return db.Query<Order>(); }
        }

        public IQueryable<OrderDetail> OrderDetails
        {
            get { return db.Query<OrderDetail>(); }
        }

        public IQueryable<Product> Products
        {
            get { return db.Query<Product>(); }
        }

        public IQueryable<ActiveProduct> ActiveProducts
        {
            get { return db.Query<ActiveProduct>(); }
        }

        public IQueryable<DiscontinuedProduct> DiscontinuedProducts
        {
            get { return db.Query<DiscontinuedProduct>(); }
        }

        public IQueryable<Region> Regions
        {
            get { return db.Query<Region>(); }
        }

        public IQueryable<Shipper> Shippers
        {
            get { return db.Query<Shipper>(); }
        }

        public IQueryable<Supplier> Suppliers
        {
            get { return db.Query<Supplier>(); }
        }

        public IQueryable<Territory> Territories
        {
            get { return db.Query<Territory>(); }
        }
    }
}
