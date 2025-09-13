
using Api.Entities;
using Api.Entities.Experiment;
using Microsoft.EntityFrameworkCore;

namespace Api;

public class StoreDbContext(DbContextOptions<StoreDbContext> options)
    : DbContext(options)
{
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<UnitConversion> UnitConversions { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<FeatureValue> FeatureValues { get; set; }
    public DbSet<ProductFeature> ProductFeatures { get; set; }
    public DbSet<SaleReturnItem> SaleReturnItems { get; set; }

    public DbSet<SaleReturn> SaleReturns { get; set; }

    public DbSet<Sale> Sales { get; set; }

    public DbSet<SaleItem> SaleItems { get; set; }

    public DbSet<Purchase> Purchases { get; set; }

    public DbSet<PurchaseItem> PurchaseItems { get; set; }

    public DbSet<PurchaseReturnItem> PurchaseReturnItems { get; set; }

    public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
    public DbSet<PaymentTransaction> PaymentTransaction { get; set; }
    public DbSet<Currency> Currencies { get; set; }
    public DbSet<Customer> Customers { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
                   .HasOne(c => c.Parent)
                   .WithMany(c => c.Children)
                   .HasForeignKey(c => c.ParentId);

        modelBuilder.Entity<Product>()
                  .HasOne(p => p.Category)
                  .WithMany(c => c.Products)
                  .HasForeignKey(p => p.CategoryId);

        modelBuilder.Entity<ProductFeature>()
                   .HasKey(pf => new { pf.ProductId, pf.FeatureValueId });

        modelBuilder.Entity<ProductFeature>()
                    .HasOne(pf => pf.Product)
                    .WithMany(p => p.ProductFeatures)
                    .HasForeignKey(pf => pf.ProductId);

        modelBuilder.Entity<ProductFeature>()
            .HasOne(pf => pf.FeatureValue)
            .WithMany(fv => fv.ProductFeatures)
            .HasForeignKey(pf => pf.FeatureValueId);

        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(p => p.BrandId)
            .HasMaxLength(100);
    }
}