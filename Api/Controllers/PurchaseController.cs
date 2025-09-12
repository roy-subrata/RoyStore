


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

        var _purchases = dbContext.Purchases.AsQueryable();
        if (!string.IsNullOrWhiteSpace(productQuery.Search))
        {
            _purchases = _purchases.Where(p => p.PurchaseNumber.Contains(productQuery.Search));
        }

        var totalCount = _purchases.Count();

        var purchases = await _purchases
            .OrderByDescending(p => p.PurchaseDate)
            .Include(p => p.Items)
            .Include(p => p.Supplier)
            .Skip((productQuery.Page - 1) * productQuery.PageSize)
            .Take(productQuery.PageSize)
            .Select(p => new GetPurchaseResponse(
                p.Id,
                p.PurchaseNumber,
                "",
                p.PurchaseDate,
                new EntityRef(p.Supplier.Id, p.Supplier.Name)
            )).ToListAsync();

        var result = new Paging<GetPurchaseResponse>
        {
            Data = [],
            Total = totalCount,
            Page = productQuery.Page,
            PageSize = productQuery.PageSize
        };
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating new purchase with request: {@Request}", request);

        var supplier = dbContext.Suppliers.Find(request.SupplierId);
        if (supplier == null)
        {
            logger.LogWarning("Supplier with ID {SupplierId} not found", request.SupplierId);
            return BadRequest($"Supplier with ID {request.SupplierId} not found.");
        }

        // var purchase = new Purchase
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     PurchaseNumber = request.PurchaseNo,
        //     SupplierId = request.SupplierId,
        //     PurchaseDate = DateTime.Parse(request.PurchaseDate),
        //     DeliveryCharge = request.DeliveryCharge,
        //     VatAmount = request.VatAmount,
        //     TaxAmount = request.TaxAmount,
        //     DiscountAmount = request.DiscountAmount,
        //     DueAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice) + request.DeliveryCharge + request.VatAmount + request.TaxAmount - request.DiscountAmount - request.PaidAmount,
        //     TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice) + request.DeliveryCharge + request.VatAmount + request.TaxAmount - request.DiscountAmount,
        //     PaidAmount = request.PaidAmount,
        //     Status = request.Status,
        //     Items = request.Items.Select(i => new PurchaseItem
        //     {
        //         Id = Guid.NewGuid().ToString(),
        //         ProductId = i.Id,
        //         Quantity = i.Quantity,
        //         RemainingQuantity = i.Quantity,
        //         UnitPrice = i.UnitPrice,
        //         PurchaseDate = DateTime.Parse(request.PurchaseDate)
        //     }).ToList()
        // };

        // dbContext.Purchases.Add(purchase);
        // await dbContext.SaveChangesAsync();

        // var response = new GetPurchase(
        //     purchase.Id,
        //     purchase.PurchaseNumber,
        //     purchase.Status.convertToString(),
        //     purchase.PurchaseDate,
        //     new EntityRef(supplier.Id, supplier.Name),
        //     purchase.Items.Sum(i => i.Quantity * i.UnitPrice),
        //     purchase.TotalAmount,
        //     purchase.PaidAmount,
        //     purchase.DueAmount,
        //     purchase.DeliveryCharge,
        //     purchase.VatAmount,
        //     purchase.TaxAmount,
        //     purchase.DiscountAmount,
        //     purchase.Items.Select(i => new GetPurchaseItem(
        //         i.ProductId,
        //         i.Product?.Name,
        //         i.Product?.LocalName,
        //         i.Product?.PartNo ?? string.Empty,
        //        i.Quantity,
        //        i.RemainingQuantity,
        //        i.UnitPrice
        //     )).ToList()
        // );

        // logger.LogInformation("Purchase created with ID {PurchaseId}", purchase.Id);
        await Task.FromResult(request);
        return Ok(request);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePurchase(string id, [FromBody] CreatePurchaseRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating purchase with ID {PurchaseId} and request: {@Request}", id, request);

        var purchase = await dbContext.Purchases
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        var supplier = await dbContext.Suppliers.FindAsync(new object[] { request.SupplierId }, cancellationToken);
        if (supplier == null)
        {
            logger.LogWarning("Supplier with ID {SupplierId} not found", request.SupplierId);
            return BadRequest($"Supplier with ID {request.SupplierId} not found.");
        }

        // purchase.PurchaseNumber = request.PurchaseNo;
        // purchase.SupplierId = request.SupplierId;
        // purchase.PurchaseDate = DateTime.Parse(request.PurchaseDate);
        // purchase.DeliveryCharge = request.DeliveryCharge;
        // purchase.VatAmount = request.VatAmount;
        // purchase.TaxAmount = request.TaxAmount;
        // purchase.DiscountAmount = request.DiscountAmount;
        // purchase.DueAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice) + request.DeliveryCharge + request.VatAmount + request.TaxAmount - request.DiscountAmount - request.PaidAmount;
        // purchase.TotalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice) + request.DeliveryCharge + request.VatAmount + request.TaxAmount - request.DiscountAmount;
        // purchase.PaidAmount = request.PaidAmount;
        // purchase.Status = request.Status;

        // Update items
        dbContext.PurchaseItems.RemoveRange(purchase.Items);
        // purchase.Items = request.Items.Select(i => new PurchaseItem
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     ProductId = i.Id,
        //     Quantity = i.Quantity,
        //     RemainingQuantity = i.Quantity,
        //     UnitPrice = i.UnitPrice,
        //     PurchaseDate = DateTime.Parse(request.PurchaseDate)
        // }).ToList();

        await dbContext.SaveChangesAsync(cancellationToken);

        // var response = new GetPurchase(
        //     purchase.Id,
        //     purchase.PurchaseNumber,
        //      purchase.Status.convertToString(),
        //     purchase.PurchaseDate,
        //     new EntityRef(supplier.Id, supplier.Name),
        //     purchase.Items.Sum(i => i.Quantity * i.UnitPrice),
        //     purchase.TotalAmount,
        //     purchase.PaidAmount,
        //     purchase.DueAmount,
        //     purchase.DeliveryCharge,
        //     purchase.VatAmount,
        //     purchase.TaxAmount,
        //     purchase.DiscountAmount,
        //     purchase.Items.Select(i => new GetPurchaseItem(
        //         i?.ProductId,
        //         i?.Product?.Name,
        //         i?.Product?.LocalName,
        //         i?.Product?.PartNo,
        //        i.Quantity,
        //        i.RemainingQuantity,
        //        i.UnitPrice
        //     )).ToList()
        // );
        logger.LogInformation("Purchase with ID {PurchaseId} updated successfully", purchase.Id);
        return Ok(request);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching purchase with ID {PurchaseId}", id);

        var purchase = await dbContext.Purchases
            .Include(p => p.Items)
            .ThenInclude(i => i.Product)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        // var response = new GetPurchase(
        //     purchase.Id,
        //     purchase.PurchaseNumber,
        //     purchase.Status.convertToString(),
        //     purchase.PurchaseDate,
        //     new EntityRef(purchase.SupplierId, purchase.Supplier.Name),
        //     0,
        //     purchase.TotalAmount,
        //     purchase.PaidAmount,
        //     purchase.DueAmount,
        //     purchase.DeliveryCharge,
        //     purchase.VatAmount,
        //     purchase.TaxAmount,
        //     purchase.DiscountAmount,
        //     purchase.Items.Select(i => new GetPurchaseItem(
        //         i.ProductId,
        //         i.Product.Name,
        //         i.Product.LocalName,
        //         i.Product.PartNo,
        //        i.Quantity,
        //        i.RemainingQuantity,
        //        i.UnitPrice
        //     )).ToList()
        // );

        return Ok();
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

    // [HttpPut("{id}/status")]
    // public async Task<IActionResult> UpdateStatus(string id, [FromBody] StatusUpdateRequest request, CancellationToken cancellationToken)
    // {
    //     logger.LogInformation("Updating status of purchase with ID {PurchaseId} to {Status}", id, request.Status);

    //     var purchase = await dbContext.Purchases.FindAsync(new object[] { id }, cancellationToken);
    //     if (purchase == null)
    //     {
    //         logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
    //         return NotFound();
    //     }

    //     //purchase.Status = request.Status;
    //     await dbContext.SaveChangesAsync(cancellationToken);

    //     logger.LogInformation("Status of purchase with ID {PurchaseId} updated to {Status}", id, request.Status);
    //     return NoContent();
    // }
    [HttpPut("{id}/pay")]
    public async Task<IActionResult> MakePayment(string id, [FromBody] double amount, CancellationToken cancellationToken)
    {
        logger.LogInformation("Making payment of {Amount} for purchase with ID {PurchaseId}", amount, id);

        var purchase = await dbContext.Purchases.FindAsync(new object[] { id }, cancellationToken);
        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        if (amount <= 0)
        {
            logger.LogWarning("Invalid payment amount: {Amount}", amount);
            return BadRequest("Payment amount must be greater than zero.");
        }

        // if (amount > purchase.DueAmount)
        // {
        //     logger.LogWarning("Payment amount {Amount} exceeds due amount {DueAmount} for purchase with ID {PurchaseId}", amount, purchase.DueAmount, id);
        //     return BadRequest("Payment amount exceeds due amount.");
        // }

        //  purchase.PaidAmount += amount;
        //   purchase.DueAmount -= amount;

        // if (purchase.DueAmount == 0)
        // {
        //     purchase.Status = Status.Paid;
        // }
        // else if (purchase.DueAmount > 0 && purchase.PaidAmount > 0)
        // {
        //     purchase.Status = Status.Partial;
        // }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Payment of {Amount} made for purchase with ID {PurchaseId}. New due amount is {DueAmount}", amount, id);
        return NoContent();
    }

    [HttpPut("{id}/refund")]
    public async Task<IActionResult> ProcessRefund(string id, [FromBody] double amount, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing refund of {Amount} for purchase with ID {PurchaseId}", amount, id);

        var purchase = await dbContext.Purchases.FindAsync(new object[] { id }, cancellationToken);
        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        if (amount <= 0)
        {
            logger.LogWarning("Invalid refund amount: {Amount}", amount);
            return BadRequest("Refund amount must be greater than zero.");
        }

        // if (amount > purchase.PaidAmount)
        // {
        //     logger.LogWarning("Refund amount {Amount} exceeds paid amount {PaidAmount} for purchase with ID {PurchaseId}", amount, purchase.PaidAmount, id);
        //     return BadRequest("Refund amount exceeds paid amount.");
        // }

        // purchase.PaidAmount -= amount;
        // purchase.DueAmount += amount;

        // if (purchase.DueAmount > 0)
        // {
        //     purchase.Status = Status.Partial;
        // }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Refund of {Amount} processed for purchase with ID {PurchaseId}. New paid amount is {PaidAmount}", amount, id);
        return NoContent();
    }

    [HttpPut("{id}/retrun")]
    public async Task<IActionResult> ReturnPurchase(string id, [FromBody] List<CreatePurchaseItem> returnItems, CancellationToken cancellationToken)
    {
        logger.LogInformation("Processing return for purchase with ID {PurchaseId} for items: {@ReturnItems}", id, returnItems);

        var purchase = await dbContext.Purchases
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (purchase == null)
        {
            logger.LogWarning("Purchase with ID {PurchaseId} not found", id);
            return NotFound();
        }

        foreach (var returnItem in returnItems)
        {
            var item = purchase.Items.FirstOrDefault(i => i.ProductId == returnItem.Id);
            if (item == null)
            {
                logger.LogWarning("Product with ID {ProductId} not found in purchase with ID {PurchaseId}", returnItem.Id, id);
                return BadRequest($"Product with ID {returnItem.Id} not found in this purchase.");
            }

            // if (returnItem.Quantity <= 0 || returnItem.Quantity > item.RemainingQuantity)
            // {
            //     logger.LogWarning("Invalid return quantity {Quantity} for product with ID {ProductId} in purchase with ID {PurchaseId}", returnItem.Quantity, returnItem.Id, id);
            //     return BadRequest($"Invalid return quantity for product with ID {returnItem.Id}.");
            // }

            //  item.RemainingQuantity -= returnItem.Quantity;
            // Optionally, you can track returned quantity separately
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Return processed for purchase with ID {PurchaseId}", id);
        return NoContent();
    }

}


public record GetPurchaseResponse(
    string Id,
    string PurchaseNo,
    string Status,
    DateTime PurchaseDate,
    EntityRef Supplier
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
    string SupplierId,
    string PurchaseDate,
    double DeliveryCharge,
    double VatAmount,
    double TaxAmount,
    double DiscountAmount,
    double PaidAmount,
    List<CreatePurchaseItem> Items
    );

public record CreatePurchaseItem(
    string Id,
    int Quantity,
    double UnitPrice
    );

