using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api;

public class StoreDbContext(DbContextOptions<StoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    
    public DbSet<SaleReturnItem> SaleReturnItems { get; set; }
    
    public DbSet<SaleReturn> SaleReturns { get; set; }
    
    public DbSet<Sale> Sales { get; set; }
    
    public DbSet<SaleItem> SaleItems { get; set; }
    
    public DbSet<Purchase> Purchases { get; set; }
    
    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    
    public DbSet<StockMovement> StockMovements { get; set; }
    
    public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }
    
    public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
    
    public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
    public DbSet<Currency>  Currencies { get; set; } 
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    // public DbSet<ProductVariation> ProductVariations { get; set; }
    
    public DbSet<ProductAttribute> ProductVariationAttributes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(p => p.BrandId)
            .HasMaxLength(100);
        
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);

            entity.HasOne(e => e.PurchaseItem)
                .WithMany()
                .HasForeignKey(e => e.PurchaseItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.MovementType)
                .HasConversion<string>();

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.Date)
                .IsRequired();
        });

        // ProductVariation config
        // modelBuilder.Entity<ProductVariation>()
        //     .Property(v => v.UnitId)
        //     .IsRequired()
        //     .HasMaxLength(50);
        //
        // // Optional uniqueness: same PartNo allowed if Brand or Attributes differ
        // // This example just adds an index for faster queries
        // modelBuilder.Entity<ProductVariation>()
        //     .HasIndex(v => new { v.PartNo, v.ProductId })
        //     .IsUnique(false);

        // ProductVariationAttribute config
        modelBuilder.Entity<ProductAttribute>()
            .Property(a => a.AttributeName)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<ProductAttribute>()
            .Property(a => a.AttributeValue)
            .HasMaxLength(200);
    }
}