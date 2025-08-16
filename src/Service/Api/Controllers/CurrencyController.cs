using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrencyController(
    ILogger<CategoryController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CurrencyQuery request)
    {
        var query = context.Currencies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c =>
                c.CurrencyCode.ToLower().Contains(request.Search.ToLower()) ||
                c.CurrencyName.ToLower().Contains(request.Search.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var currency = await query
            .OrderBy(b => b.CurrencyName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x =>
                new GetCurrency(x.Id, x.CurrencyName, x.CurrencyCode, x.ExchangeRateToBase, x.IsBaseCurrency))
            .ToListAsync();

        var pageResult = new Paging<GetCurrency>()
        {
            Data = currency,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = totalCount
        };
        return Ok(pageResult);
    }

    [HttpGet("{id}")]
   public async Task<ActionResult> Get(string id)
    {
        var find = await context.Currencies.FindAsync(id);
        if (find is null)
            return NotFound();
        var currency = new GetCurrency(find.Id, find.CurrencyName, find.CurrencyCode, find.ExchangeRateToBase,
            find.IsBaseCurrency);
        return Ok(currency);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCurrency request)
    {
        var currency = new Currency()
        {
            Id = Guid.NewGuid().ToString(),
            CurrencyCode = request.CurrencyCode,
            CurrencyName = request.CurrencyName,
            ExchangeRateToBase = request.ExchangeRateToBase,
            IsBaseCurrency = request.IsBaseCurrency
        };
        await context.Currencies.AddAsync(currency);
        await context.SaveChangesAsync();
        return Ok(currency);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCurrency request)
    {
        var find = await context.Currencies.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.CurrencyCode = request.CurrencyCode;
        find.CurrencyName = request.CurrencyName;
        find.ExchangeRateToBase = request.ExchangeRateToBase;
        find.IsBaseCurrency = request.IsBaseCurrency;
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Currencies.FindAsync(id);
        if (find is null)
            return NotFound();
        
        context.Currencies.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}