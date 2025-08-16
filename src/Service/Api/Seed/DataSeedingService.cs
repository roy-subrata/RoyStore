using Api.Entities;

namespace Api.Seed;

public class DataSeedingService(IServiceScopeFactory scopeFactory)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
        if (!context.Categories.Any())
        {
            await context.Categories.AddRangeAsync(
                new Category { Id = "1b29fc40-ca47-1067-b31d-00dd010662da", Name = "WindShield" },
                new Category { Id = "2b29fc40-ca47-1067-b31d-00dd010662da", Name = "Glass" }
            );
        }

        if (!context.Brands.Any())
        {
            await context.Brands.AddRangeAsync(
                new Brand { Id = "3b29fc40-ca47-1067-b31d-00dd010662da", Name = $"Xyz" },
                new Brand { Id = "4b29fc40-ca47-1067-b31d-00dd010662da", Name = $"SUNNY" }
            );
        }

        if (!context.Units.Any())
        {
            await context.Units.AddAsync(new Unit()
            {
                Id = "8b29fc40-ca47-1067-b31d-00dd010662da",
                Name = "Pc",
            });
        }

        if (!context.Customers.Any())
        {
            await context.Customers.AddRangeAsync(new Customer
                {
                    Id = "005866fc-5953-40d4-908c-2394c493f2d6",
                    Name = "Customer",
                    Phone = "01716625366",
                    Address = "Dhaka",
                    Email = "dhaka@gmail.com"
                }
            );
        }

        if (!context.Suppliers.Any())
        {
            await context.Suppliers.AddRangeAsync(new Supplier()
            {
                Id = "025866fc-5953-40d4-908c-2394c493f2d6",
                Name = "Supplier Name",
                Email = "customer@gmail.com",
                Phone = "01716625366",
                Address = "Dhaka",
            });
        }

        if (!context.Products.Any())
        {
            await context.Products.AddRangeAsync(
                new Product()
                {
                    Id = "3425866fc-5953-40d4-908c-2394c493f2d6",
                    Name = "Glass",
                    LocalName = "Glass",
                    PartNo = "8787-090",
                    BrandId = "3b29fc40-ca47-1067-b31d-00dd010662da",
                    CategoryId = "1b29fc40-ca47-1067-b31d-00dd010662da",
                    Attributes =
                    [
                        new ProductAttribute()
                        {
                            Id = "9425866fc-5953-40d4-908c-2394c493f2d6",
                            AttributeName = "Side",
                            AttributeValue = "Right"
                        },
                        new ProductAttribute()
                        {
                            Id = "8425866fc-5953-40d4-908c-2394c493f2d6",
                            AttributeName = "Color",
                            AttributeValue = "Red"
                        }
                    ],
                    Notes = "N/A"
                });
        }

        if (!context.Purchases.Any())
        {
            var purchase = new Purchase()
            {
                Id = "025866fc-5953-40d4-908c-2394c493f2f4",
                PurchaseNumber = "PO-12",
                PaidAmount = 0,
                TotalAmount = 120,
                PurchaseDate = DateTime.Now,
                Status = Status.Pending,
                SupplierId = "025866fc-5953-40d4-908c-2394c493f2d6",
            };
            await context.Purchases.AddAsync(purchase, cancellationToken);
        }

        if (!context.PurchaseItems.Any())
        {
            var purchaseItem = new PurchaseItem()
            {
                Id = "025866fc-5953-40d4-908c-2394c493f290",
                ProductId = "3425866fc-5953-40d4-908c-2394c493f2d6",
                PurchaseId = "025866fc-5953-40d4-908c-2394c493f2f4",
                RemainingQuantity = 10,
                UnitPrice = 12,
                Quantity = 10,
                PurchaseDate = DateTime.Now,
            };
            await context.PurchaseItems.AddAsync(purchaseItem, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}