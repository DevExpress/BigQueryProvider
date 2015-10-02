using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Xunit;

namespace DevExpress.DataAccess.BigQuery.EntityFarmework7.Tests {
    public class TestDataEntities : DbContext {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<OrderHeader> OrderHeaders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseBigQuery(ConfigurationManager.ConnectionStrings["bigqueryConnectionStringOAuth"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Product>(entity => {
                entity.ToTable("Product");
                entity.Key(e => e.ProductID);
                entity.Property(e => e.ProductName).MaxLength(50);
                entity.Property(e => e.SellStartDate).Required();
                entity.Property(e => e.SellEndDate).Required(false);
            });
            modelBuilder.Entity<OrderHeader>(entity => {
                entity.ToTable("OrderHeader");
                entity.Key(e => e.OrderID);
                entity.Property(e => e.OrderDate).Required();
                entity.Property(e => e.Description).MaxLength(50);
            });
            modelBuilder.Entity<OrderDetail>(entity => {
                entity.ToTable("OrderDetail");
                entity.Reference(e => e.Product).InverseCollection(r => r.Details).PrincipalKey(p => p.ProductID).ForeignKey(f => f.ProductID);
                entity.Reference(e => e.Header).InverseCollection(r => r.Details).PrincipalKey(h => new {h.OrderID, h.OrderDate}).ForeignKey(d=> new {d.OrderID, d.OrderDate});
            });
        }
    }

    public class OrderDetail {
        public long OrderDetailID { get; set; }
        public long OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public long OrderQty { get; set; }
        public float UnitPrice { get; set; }
        public long ProductID { get; set; }

        public virtual Product Product { get; set; }
        public virtual OrderHeader Header { get; set; }
    }

    public class OrderHeader {
        public OrderHeader() {
            Details = new HashSet<OrderDetail>();
        }

        public long OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public long Status { get; set; }
        public string Description { get; set; }

        public virtual ICollection<OrderDetail> Details { get; private set; }
    }

    public class Product {
        public Product() {
            Details = new HashSet<OrderDetail>();
        }

        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public float ListPrice { get; set; }
        public DateTime SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }

        public virtual ICollection<OrderDetail> Details { get; private set; }
    }

    public class EntityTests {
        [Fact]
        public void EntityTest() {
            TestDataEntities context = new TestDataEntities();
            List<Product> products = context.Products.ToList();
            Assert.Equal(13, products.Count);
            List<OrderDetail> details = context.OrderDetails.ToList();
            Assert.Equal(41, details.Count);
            List<OrderHeader> orders = context.OrderHeaders.ToList();
            Assert.Equal(13, orders.Count);
        }

    }
}
