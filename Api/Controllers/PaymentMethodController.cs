

using Api;
using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PaymentMethodController(
    ILogger<PaymentMethodController> logger,
     StoreDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> FindAll([FromQuery] Query query)
    {
        logger.LogInformation("Get PaymentMethods");

        var queryable = context.PaymentMethods
            .AsNoTracking();
        //  .Include(x => x.PaymentType); // ✅ eager load PaymentType

        if (!string.IsNullOrEmpty(query.Search))
        {
            queryable = queryable.Where(q => q.ProviderName.ToLower().Contains(query.Search.ToLower()));
        }

        var totalCount = await queryable.CountAsync();

        var types = await queryable
            .OrderBy(b => b.ProviderName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new GetPaymentMethodRespone(
                x.Id,
                x.ProviderName,
                x.AccountNo,
                x.AccountOwner,
                x.IsActive)) // ✅ use PaymentMethod.IsActive, not PaymentType.IsActive
            .ToListAsync();

        return Ok(new Paging<GetPaymentMethodRespone>
        {
            Data = types,
            Page = query.Page,
            PageSize = query.PageSize,
            Total = totalCount
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(string id)
    {
        var find = await context.PaymentMethods
        .FirstOrDefaultAsync(x => x.Id == id);

        if (find is null)
            return NotFound();
        var response = new GetPaymentMethodRespone(find.Id, find.ProviderName, find.AccountNo, find.AccountOwner, find.IsActive);
        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePaymentMethod request)
    {
        var method = new PaymentMethod
        {
            Id = Guid.NewGuid().ToString(),
            AccountNo = request.AccountNo,
            AccountOwner = request.AccountOwner,
            ProviderName = request.ProviderName,
            IsActive = request.IsActive
        };

        await context.PaymentMethods.AddAsync(method);
        await context.SaveChangesAsync();


        logger.LogInformation("PaymentMethod {Id} created", method.Id);

        return CreatedAtAction(nameof(GetById), new { id = method.Id },
            new GetPaymentMethodRespone(method.Id, method.ProviderName, method.AccountNo, method.AccountOwner, method.IsActive));

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdatePaymentMethod request)
    {
        var type = await context.PaymentMethods.FindAsync(id);
        if (type is null)
        {
            return BadRequest($"Payment  with {id} not found.");
        }
        var method = await context.PaymentMethods.FindAsync(id);
        if (method is null)
            return NotFound();

        method.IsActive = request.IsActive;
        method.IsActive = request.IsActive;
        method.IsActive = request.IsActive;
        await context.SaveChangesAsync();

        logger.LogInformation("PaymentMethod {Id} updated", id);

        return Ok(new GetPaymentMethodRespone(method.Id, method.ProviderName, method.AccountNo, method.AccountOwner, method.IsActive));
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.PaymentMethods.FindAsync(id);
        if (find is null)
            return NotFound();

        context.PaymentMethods.Remove(find);
        await context.SaveChangesAsync();
        logger.LogInformation("PaymentMethod {Id} deleted", id);
        return NoContent();
    }
}

public record GetPaymentMethodRespone(string Id, string ProviderName, string AccountNo, string AccountOwner, bool IsActive);

public record CreatePaymentMethod(string ProviderName, string AccountNo, string AccountOwner, bool IsActive);

public record UpdatePaymentMethod(string ProviderName, string AccountNo, string AccountOwner, bool IsActive);