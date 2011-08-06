using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modl;
using Modl.Attributes;

namespace ExampleModel.Northwind
{
    [Table("Categories")]
    [Id("CategoryId")]
    public class Category : Modl<Category>
    {
        
        
        public string CategoryName { get; set; }

        
        public string Description { get; set; }

        ////[Field(LazyLoad = true, Length = 1073741823)]
        public byte[] Picture { get; set; }

        ////[Field, Association(PairTo = "Category")]
        public List<Product> Products { get; private set; }
    }

    ////[TableMapping("Customers")]
    ////[HierarchyRoot]
    ////[KeyGenerator(null)]
    [Table("Customers")]
    [Id("CustomerId")]
    public class Customer : Modl<Customer, string>
    {
        ////[Field(Length = 5), FieldMapping("CustomerId"), Key]
        //public string Id { get; private set; }

        ////[Field(Length = 40)]
        public string CompanyName { get; set; }

        ////[Field(Length = 30)]
        public string ContactName { get; set; }

        ////[Field(Length = 30)]
        public string ContactTitle { get; set; }

        ////[Field(Length = 60)]
        public string Address { get; set; }

        ////[Field(Length = 15)]
        public string City { get; set; }

        ////[Field(Length = 15)]
        public string Region { get; set; }

        ////[Field(Length = 10)]
        public string PostalCode { get; set; }

        ////[Field(Length = 15)]
        public string Country { get; set; }

        ////[Field(Length = 24)]
        public string Phone { get; set; }

        ////[Field(Length = 24)]
        public string Fax { get; set; }

        ////[Field, Association(PairTo = "Customer")]
        public List<Order> Orders { get; private set; }

        // Constructors

        //public Customer(string id)
        //    : base(id)
        //{
        //}
    }

    ////[HierarchyRoot]
    public class Region : Modl<Region>
    {
        ////[Field, FieldMapping("RegionId"), Key]
        //public int Id { get; private set; }

        ////[Field(Length = 50)]
        public string RegionDescription { get; set; }
    }

    ////[TableMapping("Suppliers")]
    ////[HierarchyRoot]
    [Table("Suppliers")]
    [Id("SupplierId")]
    public class Supplier : Modl<Supplier>
    {
        //[Field, FieldMapping("SupplierId"), Key]
        //public int Id { get; private set; }

        //[Field(Length = 40)]
        public string CompanyName { get; set; }

        //[Field(Length = 30)]
        public string ContactName { get; set; }

        //[Field(Length = 30)]
        public string ContactTitle { get; set; }

        //[Field(Length = 60)]
        public string Address { get; set; }

        //[Field(Length = 15)]
        public string City { get; set; }

        //[Field(Length = 15)]
        public string Region { get; set; }

        //[Field(Length = 10)]
        public string PostalCode { get; set; }

        //[Field(Length = 15)]
        public string Country { get; set; }

        //[Field(Length = 24)]
        public string Phone { get; set; }

        //[Field(Length = 24)]
        public string Fax { get; set; }

        //[Field]
        public string HomePage { get; set; }
    }

    //[TableMapping("Shippers")]
    //[HierarchyRoot]
    [Table("Shippers")]
    [Id("ShipperId")]
    public class Shipper : Modl<Shipper>
    {
        //[Field, FieldMapping("ShipperId"), Key]
        //public int Id { get; private set; }

        //[Field(Length = 40)]
        public string CompanyName { get; set; }

        //[Field(Length = 24)]
        public string Phone { get; set; }
    }

    public class ActiveProduct : Inherits<Product, ActiveProduct> //Product, IModl<ActiveProduct> // Inherits<Product, ActiveProduct> // Modl<ActiveProduct> //Product
    {
        public ActiveProduct()
        { 
            
        }
    }

    public class DiscontinuedProduct : Inherits<Product, DiscontinuedProduct> // Modl<DiscontinuedProduct>
    {
        public DiscontinuedProduct()
        { 
            
        }
    }

    //[TableMapping("Products")]
    //[HierarchyRoot(InheritanceSchema.SingleTable)]
    [Table("Products")]
    [Id("ProductId")]
    public class Product : Modl<Product>
    {
        //[Field, FieldMapping("ProductId"), Key]
        //public int Id { get; private set; }

        //[Field(Length = 40)]
        public string ProductName { get; set; }

        //[Field]
        public Supplier Supplier { get; set; }

        //[Field]
        public Category Category { get; set; }

        //[Field(Length = 20)]
        public string QuantityPerUnit { get; set; }

        //[Field]
        public decimal UnitPrice { get; set; }

        //[Field]
        public short UnitsInStock { get; set; }

        //[Field]
        public short UnitsOnOrder { get; set; }

        //[Field]
        public short ReorderLevel { get; set; }

        //[Field]
        public bool Discontinued { get; set; }
    }

    //[TableMapping("Employees")]
    //[HierarchyRoot]
    [Table("Employees")]
    [Id("Id")]
    public class Employee : Modl<Employee>
    {
        //[Field, Key]
        //public int Id { get; private set; }

        //[Field(Length = 20)]
        public string LastName { get; set; }

        //[Field(Length = 10)]
        public string FirstName { get; set; }

        //[Field(Length = 30)]
        public string Title { get; set; }

        //[Field(Length = 25)]
        public string TitleOfCourtesy { get; set; }

        //[Field]
        public DateTime? BirthDate { get; set; }

        //[Field]
        public DateTime? HireDate { get; set; }

        //[Field(Length = 60)]
        public string Address { get; set; }

        //[Field(Length = 15)]
        public string City { get; set; }

        //[Field(Length = 15)]
        public string Region { get; set; }

        //[Field(Length = 10)]
        public string PostalCode { get; set; }

        //[Field(Length = 15)]
        public string Country { get; set; }

        //[Field(Length = 24)]
        public string HomePhone { get; set; }

        //[Field(Length = 4)]
        public string Extension { get; set; }

        //[Field(Length = 1073741823)]
        public byte[] Photo { get; set; }

        //[Field(Length = 255)]
        public string PhotoPath { get; set; }

        //[Field]
        public string Notes { get; set; }

        //[Field]
        public Employee ReportsTo { get; set; }

        //[Field, Association(PairTo = "ReportsTo")]
        public List<Employee> ReportingEmployees { get; set; }

        //[Field, Association(PairTo = "Employee")]
        public List<Order> Orders { get; private set; }

        //[Field]
        public List<Territory> Territories { get; private set; }
    }

    //[TableMapping("Territories")]
    //[HierarchyRoot]
    //[KeyGenerator(null)]
    [Table("Territories")]
    [Id("TerritoryId")]
    public class Territory : Modl<Territory, string>
    {
        //[Field(Length = 20), FieldMapping("TerritoryId"), Key]
        //public string Id { get; private set; }

        //[Field(Length = 50)]
        public string TerritoryDescription { get; set; }

        //[Field]
        public Region Region { get; set; }

        //[Field, Association(PairTo = "Territories")]
        public List<Employee> Employees { get; private set; }

        // Constructors

        //public Territory(string id)
        //    : base(id)
        //{
        //}
    }

    //[TableMapping("Orders")]
    //[HierarchyRoot]
    [Table("Orders")]
    [Id("OrderId")]
    public class Order : Modl<Order>
    {
        //[Field, FieldMapping("OrderId"), Key]
        //public int Id { get; private set; }

        //[Field]
        public Customer Customer { get; set; }

        //[Field]
        public Employee Employee { get; set; }

        //[Field]
        public DateTime? OrderDate { get; set; }

        //[Field]
        public DateTime? RequiredDate { get; set; }

        //[Field]
        public DateTime? ShippedDate { get; set; }

        //[Field]
        public Shipper ShipVia { get; set; }

        //[Field]
        public decimal Freight { get; set; }

        //[Field(Length = 60)]
        public string ShipName { get; set; }

        //[Field(Length = 60)]
        public string ShipAddress { get; set; }

        //[Field(Length = 15)]
        public string ShipCity { get; set; }

        //[Field(Length = 15)]
        public string ShipRegion { get; set; }

        //[Field(Length = 10)]
        public string ShipPostalCode { get; set; }

        //[Field(Length = 15)]
        public string ShipCountry { get; set; }
    }

    //[TableMapping("OrderDetails")]
    //[HierarchyRoot]
    //[KeyGenerator(null)]
    [Table("OrderDetails")]
    //[Id("CategoryId")]
    public class OrderDetail : Modl<OrderDetail, Order>
    {
        //[Field, Key(0)]
        public Order Order { get; private set; }

        //[Field, Key(1)]
        public Product Product { get; private set; }

        //[Field]
        public decimal UnitPrice { get; set; }

        //[Field]
        public short Quantity { get; set; }

        //[Field]
        public float Discount { get; set; }


        // Constructors

        //public OrderDetail(Order order, Product product)
        //    : base(order, product)
        //{
        //}
    }

}
