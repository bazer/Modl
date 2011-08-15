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
            get { return Category.Query(db); }
        }

        public IQueryable<Customer> Customers
        {
            get { return Customer.Query(db); ; }
        }

        public IQueryable<Employee> Employees
        {
            get { return Employee.Query(db); }
        }

        public IQueryable<Order> Orders
        {
            get { return Order.Query(db); }
        }

        public IQueryable<OrderDetail> OrderDetails
        {
            get { return OrderDetail.Query(db); }
        }

        public IQueryable<Product> Products
        {
            get { return Product.Query(db); }
        }

        public IQueryable<ActiveProduct> ActiveProducts
        {
            get { return ActiveProduct.Query(db); }
        }

        public IQueryable<DiscontinuedProduct> DiscontinuedProducts
        {
            get { return DiscontinuedProduct.Query(db); }
        }

        public IQueryable<Region> Regions
        {
            get { return Region.Query(db); }
        }

        public IQueryable<Shipper> Shippers
        {
            get { return Shipper.Query(db); }
        }

        public IQueryable<Supplier> Suppliers
        {
            get { return Supplier.Query(db); }
        }

        public IQueryable<Territory> Territories
        {
            get { return Territory.Query(db); }
        }
    }
}
