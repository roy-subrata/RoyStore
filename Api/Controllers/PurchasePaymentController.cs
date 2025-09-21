using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchasePaymentController(
    ILogger<PurchasePaymentController> logger,
    StoreDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPayment([FromQuery] PaymentQuery query)
    {
        logger.LogInformation("Fetching payments for PurchaseId: {PurchaseId}",
            query.PurchaseId);

        var queryable = dbContext.PaymentTransaction
            .Include(x => x.Purchase)
            .Where(x => x.PartyType == PartyType.Customer && x.PurchaseId == query.PurchaseId)
            .AsNoTracking();

        var total = await queryable.CountAsync();
        var payments = await queryable
            .Select(x => new GetPurchasePaymentResponse(
                x.Id,
                x.Purchase.Id,
                x.PaymentDate,
                x.PartyId,
                x.PaymentMethodId,
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

    [HttpPost]
    public async Task<IActionResult> CreatePurchasePayment([FromBody] CreatePurchasePaymentRequest request)
    {
        logger.LogInformation("Adding new payment for PurchaseId: {PurchaseId}, Amount: {Amount}",
            request.PurchaseId, request.PaymentAmount);

        var purchase = await dbContext.Purchases.FindAsync(request.PurchaseId);
        if (purchase is null)
        {
            logger.LogWarning("Purchase with id {PurchaseId} not found", request.PurchaseId);
            return BadRequest($"Purchase with id {request.PurchaseId} not found");
        }

        var paymentTransaction = new PaymentTransaction()
        {
            Id = Guid.NewGuid().ToString(),
            AmountPaid = request.PaymentAmount,
            PurchaseId = request.PurchaseId,
            PaymentMethodId = request.PaymentMethodId,
            NoteRef = request.NoteRef,
            PaymentDate = request.PaymentDate,
            PartyType = PartyType.Customer,
            PartyId = request.SupplierId
        };

        dbContext.PaymentTransaction.Add(paymentTransaction);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Payment transaction {PaymentId} created successfully", paymentTransaction.Id);

        return Ok(request);
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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