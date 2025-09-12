using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CurrenciesController(
    ILogger<CurrenciesController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query query)
    {
        logger.LogInformation("Get All Currency..");
        var _currencies = context.Currencies.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            _currencies = _currencies.Where(c =>
                c.CurrencyCode.ToLower().Contains(query.Search.ToLower()) ||
                c.CurrencyName.ToLower().Contains(query.Search.ToLower()));
        }

        var totalCount = await _currencies.CountAsync();

        var currencies = await _currencies
            .OrderBy(b => b.CurrencyName)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x =>
                new GetCurrencyResponse(x.Id, x.CurrencyName, x.CurrencyCode, x.ExchangeRateToBase, x.IsBaseCurrency))
            .ToListAsync();

        var result = new Paging<GetCurrencyResponse>()
        {
            Data = currencies,
            Page = query.Page,
            PageSize = query.PageSize,
            Total = totalCount
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(string id)
    {
        var find = await context.Currencies.FindAsync(id);
        if (find is null)
            return NotFound();
        var response = new GetCurrencyResponse(find.Id, find.CurrencyName, find.CurrencyCode, find.ExchangeRateToBase,
            find.IsBaseCurrency);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCurrencyRequest request)
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
        var response = new GetCurrencyResponse(currency.Id, currency.CurrencyName, currency.CurrencyCode, currency.ExchangeRateToBase,
      currency.IsBaseCurrency);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCurrencyRequest request)
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
        var response = new GetCurrencyResponse(find.Id, find.CurrencyName, find.CurrencyCode, find.ExchangeRateToBase,
     find.IsBaseCurrency);
        return Ok(response);
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