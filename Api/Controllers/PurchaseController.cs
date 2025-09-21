
using Api.Entities;
using Api.Mapping;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/purchases")]
public class PurchaseController(
    ILogger<PurchaseReturnController> logger,
    StoreDbContext dbContext
    ) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Query productQuery, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching purchases with query: {@Query}", productQuery);

        var queryable = dbContext.Purchases.AsQueryable();
        if (!string.IsNullOrWhiteSpace(productQuery.Search))
        {
            queryable = queryable.Where(p => p.PurchaseNumber.Contains(productQuery.Search));
        }

        var totalCount = queryable.Count();

        var purchases = await queryable
            .OrderByDescending(p => p.PurchaseDate)
            .Skip((productQuery.Page - 1) * productQuery.PageSize)
            .Take(productQuery.PageSize)
            .Select(p => new GetPurchaseResponse(
                p.Id,
                p.PurchaseNumber,
                p.Status.convertToString(),
                p.PurchaseDate,
                new EntityRef(p.Supplier.Id, p.Supplier.Name),
                p.Items.Sum(x => x.UnitPrice *x.OrderedQuantity),
                p.PaymentTransactions.Sum(x => x.AmountPaid),
                p.Items.Sum(x => x.UnitPrice) - p.DiscountAmount - p.PaymentTransactions.Sum(x => x.AmountPaid)
            )).ToListAsync(cancellationToken: cancellationToken);

        var result = new Paging<GetPurchaseResponse>
        {
            Data = purchases,
            Total = totalCount,
            Page = productQuery.Page,
            PageSize = productQuery.PageSize
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching purchase with ID {PurchaseId}", id);

        var response = await dbContext.Purchases
            .Select(x => new GetPurchaseResponse(
                x.Id,
                x.PurchaseNumber,
                x.Status.convertToString(),
                x.PurchaseDate,
                new EntityRef(x.Supplier.Id, x.Supplier.Name),
                x.Items.Sum(x => x.UnitPrice * x.OrderedQuantity),
                x.PaymentTransactions.Sum(x => x.AmountPaid),
                x.Items.Sum(x => x.UnitPrice * x.OrderedQuantity) - x.DiscountAmount - x.PaymentTransactions.Sum(x => x.AmountPaid)
             ))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (response == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new purchase with request: {@Request}", request);

        var supplier = await dbContext.Suppliers.FindAsync(request.SupplierId);
        if (supplier == null)
        {
            logger.LogWarning("Supplier with ID {SupplierId} not found", request.SupplierId);
            return BadRequest($"Supplier with ID {request.SupplierId} not found.");
        }

        var purchase = new Purchase
        {
            Id = Guid.NewGuid().ToString(),
            PurchaseNumber = request.PurchaseNo,
            SupplierId = request.SupplierId,
            ShipTo = request.ShipTo,
            Vat = request.Vat,
            Tax = request.Tax,
            DiscountAmount = request.DiscountAmount,
            PurchaseDate = DateTime.Now,
            Status = PurchaseStatus.Draft,
        };

        await dbContext.Purchases.AddAsync(purchase, cancellationToken);

        foreach (var item in request.Items)
        {
            var unit = await dbContext.Units.FindAsync(item.UnitId, cancellationToken);
            if (unit is null)
                return BadRequest($"Purchase Item Unit with ID {item.UnitId} not found.");

            var purchaseItem = new PurchaseItem
            {
                Id = Guid.NewGuid().ToString(),
                PurchaseId = purchase.Id,
                UnitId = item.UnitId,
                ProductId = item.Id,
                OrderedQuantity = item.Quantity,
                UnitPrice = item.UnitPrice
            };

            await dbContext.PurchaseItems.AddAsync(purchaseItem, cancellationToken);
        }

        await dbContext.SaveChangesAsync();
        logger.LogInformation("Purchase created with ID {PurchaseId}", purchase.Id);
        return Ok(request);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePurchase(string id, [FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating purchase with ID {PurchaseId} and request: {@Request}", id, request);

        var purchase = dbContext.Purchases
            .Include(p => p.Items)
            .FirstOrDefault(p => p.Id == id);

        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        var supplier = dbContext.Suppliers.Find(request.SupplierId);
        if (supplier == null)
        {
            logger.LogWarning("Supplier with ID {SupplierId} not found", request.SupplierId);
            return BadRequest($"Supplier with ID {request.SupplierId} not found.");
        }

        purchase.SupplierId = request.SupplierId;
        purchase.PurchaseDate = request.PurchaseDate;
        purchase.ShipTo = request.ShipTo;
        purchase.Status = request.Status;
        purchase.Vat = request.Vat;
        purchase.Tax = request.Tax;
        purchase.DiscountAmount = request.DiscountAmount;

        dbContext.PurchaseItems.RemoveRange(purchase.Items);

        foreach (var item in request.Items)
        {
            var unit = dbContext.Units.Find(item.UnitId);
            if (unit is null)
                return BadRequest($"Purchase Item Unit with ID {item.UnitId} not found.");

            var find = dbContext.PurchaseItems.Find(item.Id);
            if (find is null)
            {
                var purchaseItem = new PurchaseItem
                {
                    Id = Guid.NewGuid().ToString(),
                    PurchaseId = purchase.Id,
                    UnitId = item.UnitId,
                    ProductId = item.Id,
                    OrderedQuantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                };
                dbContext.PurchaseItems.Add(purchaseItem);
            }
            else
            {
                find.PurchaseId = purchase.Id;
                find.UnitId = item.UnitId;
                find.ProductId = item.Id;
                find.OrderedQuantity = item.Quantity;
                find.UnitPrice = item.UnitPrice;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Purchase with ID {PurchaseId} updated successfully", purchase.Id);
        return Ok(request);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePurchase(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting purchase with ID {PurchaseId}", id);

        var purchase = await dbContext.Purchases
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        dbContext.PurchaseItems.RemoveRange(purchase.Items);
        dbContext.Purchases.Remove(purchase);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Purchase with ID {PurchaseId} deleted successfully", id);
        return NoContent();
    }

}



public record GetPurchaseResponse(
    string Id,
    string PurchaseNo,
    string Status,
    DateTime PurchaseDate,
    EntityRef Supplier,
    double SubTotal,
    double Paid,
    double Due
    );

public record GetPurchaseItem(
    string Id,
    string ProductName,
    string LocalName,
    string partNo,
    int Quantity,
    int RemainingQuantity,
    double UnitPrice
    );

public record CreatePurchaseRequest(
    string PurchaseNo,
    DateTime PurchaseDate,
    string SupplierId,
    PurchaseStatus Status,
    float Vat,
    float Tax,
    double DiscountAmount,
    string ShipTo,
    string NoteRef,
    List<CreatePurchaseItem> Items
    );


public record CreatePurchaseItem(
    string Id,
    double Quantity,
    string UnitId,
    double UnitPrice
    );
