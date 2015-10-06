using System.Configuration;
using Microsoft.Data.Entity;

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
                entity.Property(e => e.ListPrice).HasColumnType("float");
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
}