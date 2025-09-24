
using System.Linq;
using Api.Entities;
using Api.Mapping;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/purchase")]
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
        var purchasesRaw = await queryable
            .OrderByDescending(p => p.PurchaseDate)
            .Skip((productQuery.Page - 1) * productQuery.PageSize)
            .Take(productQuery.PageSize)
            .Select(p => new
            {
                p.Id,
                p.PurchaseNumber,
                p.Status,  // keep as enum, convert later
                p.PurchaseDate,
                Supplier = new { p.Supplier.Id, p.Supplier.Name },
                Items = p.Items.Select(i => new
                {
                    i.Id,
                    i.Product.Name,
                    i.Product.LocalName,
                    i.Product.PartNo,
                    i.OrderedQuantity,
                    i.UnitPrice
                }),
                p.Tax,
                p.Vat,
                p.DiscountAmount,
                Total = p.Items.Sum(x => x.UnitPrice * x.OrderedQuantity),
                Paid = p.PaymentTransactions.Sum(x => x.AmountPaid)
            })
            .ToListAsync(cancellationToken);

        var purchases = purchasesRaw.Select(p => new GetPurchaseResponse(
            p.Id,
            p.PurchaseNumber,
            p.Status.convertToString(),
            p.PurchaseDate,
            new EntityRef(p.Supplier.Id, p.Supplier.Name),
            p.Items.Select(i => new GetPurchaseItem(
                i.Id,
                i.Name,
                i.LocalName,
                i.PartNo,
                i.OrderedQuantity,
                i.UnitPrice
                )).ToList(),
    p.Tax,
    p.Vat,
    p.DiscountAmount,
    p.Total,
    p.Paid,
    p.Total - p.DiscountAmount - p.Paid
)).ToList();

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

        var x = await dbContext.Purchases
            .Where(p => p.Id == id)
            .Select(p => new
            {
                p.Id,
                p.PurchaseNumber,
                p.Status,
                p.PurchaseDate,
                Supplier = new { p.Supplier.Id, p.Supplier.Name },
                Items = p.Items.Select(i => new
                {
                    i.Id,
                    i.Product.Name,
                    i.Product.LocalName,
                    i.Product.PartNo,
                    i.OrderedQuantity,
                    i.UnitPrice
                }),
                p.Tax,
                p.Vat,
                p.DiscountAmount,
                Total = p.Items.Sum(i => i.UnitPrice * i.OrderedQuantity),
                Paid = p.PaymentTransactions.Sum(pt => pt.AmountPaid)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (x == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        var response = new GetPurchaseResponse(
            x.Id,
            x.PurchaseNumber,
            x.Status.convertToString(),
            x.PurchaseDate,
            new EntityRef(x.Supplier.Id, x.Supplier.Name),
            x.Items.Select(i => new GetPurchaseItem(
                i.Id,
                i.Name,
                i.LocalName,
                i.PartNo,
                i.OrderedQuantity,
                i.UnitPrice
            )).ToList(),
            x.Tax,
            x.Vat,
            x.DiscountAmount,
            x.Total,
            x.Paid,
            x.Total - x.DiscountAmount - x.Paid
        );

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

    [HttpGet("payment")]
    public async Task<IActionResult> GetPayment([FromQuery] PaymentQuery query)
    {
        logger.LogInformation("Fetching payments for PurchaseId: {PurchaseId}",
            query.PurchaseId);

        var queryable = dbContext.PaymentTransaction
            .Include(x => x.Purchase)
            .Include(x => x.PaymentMethod)
            .Where(x => x.PartyType == PartyType.Supplier && x.PurchaseId == query.PurchaseId)
            .AsNoTracking();

        var total = await queryable.CountAsync();
        var payments = await queryable
            .Select(x => new GetPurchasePaymentResponse(
                x.Id,
                x.Purchase.Id,
                x.PaymentDate,
                x.PartyId,
                x.PaymentMethod.Id,
                x.AmountPaid,
                x.NoteRef
            ))
            .ToListAsync();

        logger.LogInformation("Found {Count} payment(s)", payments.Count);
        var response = new Paging<GetPurchasePaymentResponse>()
        {
            Data = payments,
            Page = query.Page,
            PageSize = query.PageSize,
            Total = total
        };

        return Ok(response);
    }


    [HttpPost("payment")]
    public async Task<IActionResult> CreatePurchasePayment([FromBody] CreatePurchasePaymentRequest request)
    {
        logger.LogInformation("Adding new payment for PurchaseId: {PurchaseId}, Amount: {Amount}",
            request.PurchaseId, request.PaymentAmount);

        if (request.PaymentAmount <= 0)
            return BadRequest("Payment amount must be greater than zero.");

        var purchase = await dbContext.Purchases.FindAsync(request.PurchaseId);
        if (purchase is null)
        {
            logger.LogWarning("Purchase with id {PurchaseId} not found", request.PurchaseId);
            return NotFound($"Purchase with id {request.PurchaseId} not found");
        }

        var paymentTransaction = new PaymentTransaction
        {
            Id = Guid.NewGuid().ToString(),
            AmountPaid = request.PaymentAmount,
            PurchaseId = request.PurchaseId,
            PaymentMethodId = request.PaymentMethodId,
            NoteRef = request.NoteRef,
            PaymentDate = request.PaymentDate,
            PartyType = PartyType.Supplier, // or Customer depending on your domain
            PartyId = request.SupplierId
        };



        dbContext.PaymentTransaction.Add(paymentTransaction);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Payment transaction {PaymentId} created successfully", paymentTransaction.Id);

        return CreatedAtAction(nameof(CreatePurchasePayment),
            new { id = paymentTransaction.Id },
            paymentTransaction);
    }



    [HttpPut("payment/{id}")]
    public async Task<IActionResult> UpdatePurchasePayment(string id, [FromBody] CreatePurchasePaymentRequest request)
    {
        logger.LogInformation("Updating payment {PaymentId}", id);

        var paymentTransaction = await dbContext.PaymentTransaction.FindAsync(id);
        if (paymentTransaction is null)
        {
            logger.LogWarning("Payment transaction with id {PaymentId} not found", id);
            return NotFound($"Payment transaction with id {id} not found");
        }

        var purchase = await dbContext.Purchases.FindAsync(request.PurchaseId);
        if (purchase is null)
        {
            logger.LogWarning("Purchase with id {PurchaseId} not found", request.PurchaseId);
            return BadRequest($"Purchase with id {request.PurchaseId} not found");
        }

        paymentTransaction.AmountPaid = request.PaymentAmount;
        paymentTransaction.PaymentMethodId = request.PaymentMethodId;
        paymentTransaction.NoteRef = request.NoteRef;
        paymentTransaction.PaymentDate = request.PaymentDate;
        paymentTransaction.PartyId = request.SupplierId;

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Payment transaction {PaymentId} updated successfully", id);

        return Ok(request);
    }

    [HttpDelete("payment/{id}")]
    public async Task<IActionResult> DeletePurchasPayment(string id)
    {
        logger.LogInformation("Deleting payment transaction {PaymentId}", id);

        var paymentTransaction = await dbContext.PaymentTransaction.FindAsync(id);
        if (paymentTransaction is null)
        {
            logger.LogWarning("Payment transaction with id {PaymentId} not found", id);
            return NotFound($"Payment transaction with id {id} not found");
        }

        dbContext.PaymentTransaction.Remove(paymentTransaction);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Payment transaction {PaymentId} deleted successfully", id);

        return NoContent();
    }
}



public record GetPurchaseResponse(
    string Id,
    string PurchaseNo,
    string Status,
    DateTime PurchaseDate,
    EntityRef Supplier,
    List<GetPurchaseItem> Items,
    double Tax,
    double Vat,
    double DiscountAmount,
    double SubTotal,
    double Paid,
    double Due
    );

public record GetPurchaseItem(
    string Id,
    string ProductName,
    string LocalName,
    string partNo,
    double OrderedQuantity,
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

public sealed class PaymentQuery : Query
{
    public string PurchaseId { get; set; } = string.Empty;
    public string? PaymentDate { get; set; } = string.Empty;
}



public sealed record CreatePurchasePaymentRequest(
    string PurchaseId,
    string SupplierId,
    string PaymentMethodId,
    DateTime PaymentDate,
    double PaymentAmount,
    string NoteRef
);



public sealed record GetPurchasePaymentResponse(
    string Id,
    string PurchaseId,
    DateTime PaymentDate,
    string SupplierId,
    string PaymentMethodId,
    double PaymentAmount,
    string? NoteRef
);
