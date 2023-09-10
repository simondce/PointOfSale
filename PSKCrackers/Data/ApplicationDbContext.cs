using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSKCrackers.Models;

namespace PSKCrackers.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ProductType Configuration
            modelBuilder.Entity<ProductType>()
                .ToTable("ProductTypes");

            // Product Configuration
            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasOne(p => p.ProductType)
                .WithMany()
                .HasForeignKey(p => p.ProductTypeId);

            // Supplier Configuration
            modelBuilder.Entity<Supplier>()
                .ToTable("Suppliers");

            // PurchaseOrder Configuration
            modelBuilder.Entity<PurchaseOrder>()
                .ToTable("PurchaseOrders")
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            // PurchaseOrderItem Configuration
            modelBuilder.Entity<PurchaseOrderItem>()
                .ToTable("PurchaseOrderItems")
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.PurchaseOrderItems)
                .HasForeignKey(poi => poi.PurchaseOrderId);

            // Sale Configuration
            modelBuilder.Entity<Sale>()
                .ToTable("Sales")
                .HasOne(s => s.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict cascading delete

            // SaleItem Configuration
            modelBuilder.Entity<SaleItem>()
                .ToTable("SaleItems")
                .HasOne(si => si.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(si => si.SaleId);

            // Customer Configuration
            modelBuilder.Entity<Customer>()
                .ToTable("Customers");

            // InventoryItem Configuration
            modelBuilder.Entity<InventoryItem>()
                .ToTable("InventoryItems")
                .HasOne(ii => ii.Product)
                .WithMany(p => p.InventoryItems)
                .HasForeignKey(ii => ii.ProductId);
        }

    }
}
