using Api.Entities;

namespace Api.Seed;

public class DataSeedingService(IServiceScopeFactory scopeFactory)
    : IHostedService
{

    private string _supplierId = Guid.NewGuid().ToString();
    private string _customerId = Guid.NewGuid().ToString();
    private string _brandId = Guid.NewGuid().ToString();
    private string _categoryId = Guid.NewGuid().ToString();
    private string _unitId = "30a5fb2b-0907-4ace-9623-10c8fcbc8289";// Guid.NewGuid().ToString();
    private string _paymentMethodId = "fcf6a35c-4d88-4863-bc55-09bbeca9e59d";// Guid.NewGuid().ToString();
    private string _productId = Guid.NewGuid().ToString();
    private string _featureId = Guid.NewGuid().ToString();
    private string _productFeatureId = Guid.NewGuid().ToString();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
        if (!context.Categories.Any())
        {
            await context.Categories.AddRangeAsync(
                new Category { Id = _categoryId, Name = "WindShield", Description = "N/A" }
            );
        }

        if (!context.Brands.Any())
        {
            await context.Brands.AddRangeAsync(
                new Brand { Id = _brandId, Name = $"Xyz", Description = "N/A" }
            );
        }

        if (!context.Units.Any())
        {
            await context.Units.AddAsync(new Unit()
            {
                Id = _unitId,
                Name = "Piece",
                ShortCode = "Pc"
            });
        }
        if (!context.PaymentMethods.Any())
        {
            await context.PaymentMethods.AddAsync(new PaymentMethod()
            {
                Id = _paymentMethodId,
                Name = "Bikash",
                IsActive = true
            });
        }

        if (!context.Customers.Any())
        {
            await context.Customers.AddRangeAsync(new Customer
            {
                Id = _customerId,
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
                Id = _supplierId,
                Name = "Supplier Name",
                Email = "customer@gmail.com",
                Phone = "01716625366",
                Address = "Dhaka",
            });
        }

        if (!context.Features.Any())
        {
            context.Features.Add(new Entities.Experiment.Feature()
            {
                Id = _featureId,
                Name = "Display",
                IsActive = false
            });
        }

        if (!context.Products.Any())
        {
            await context.Products.AddRangeAsync(
                new Product()
                {
                    Id = _productId,
                    Name = "Glass",
                    LocalName = "Glass",
                    PartNo = "8787-090",
                    BrandId = _brandId,
                    UnitId = _unitId,
                    CategoryId = _categoryId,

                    Description = "N/A"
                });
        }

        // if (!context.Purchases.Any())
        // {
        //     var purchase = new Purchase()
        //     {
        //         Id = "025866fc-5953-40d4-908c-2394c493f2f4",
        //         PurchaseNumber = "PO-12",
        //         PaidAmount = 0,
        //         TotalAmount = 120,
        //         DeliveryCharge = 10,
        //         // DiscountAmount = 1,
        //         //  DueAmount = 12,
        //         PurchaseDate = DateTime.Now,
        //         Status = Status.Pending,
        //         SupplierId = "025866fc-5953-40d4-908c-2394c493f2d6",
        //     };
        //     await context.Purchases.AddAsync(purchase, cancellationToken);
        // }

        // if (!context.PurchaseItems.Any())
        // {
        //     var purchaseItem = new PurchaseItem()
        //     {
        //         Id = "025866fc-5953-40d4-908c-2394c493f290",
        //         ProductId = "3425866fc-5953-40d4-908c-2394c493f2d6",
        //         PurchaseId = "025866fc-5953-40d4-908c-2394c493f2f4",
        //         RemainingQuantity = 10,
        //         UnitPrice = 12,
        //         Quantity = 10,
        //         PurchaseDate = DateTime.Now,
        //     };
        //     await context.PurchaseItems.AddAsync(purchaseItem, cancellationToken);
        // }

        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}