using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitsController(
    ILogger<UnitsController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query unitQuery)
    {
        logger.LogInformation("Get Unit");
        var queryable = context.Units.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(unitQuery.Search))
        {
            queryable = queryable.Where(c => c.Name.ToLower().Contains(unitQuery.Search.ToLower()));
        }

        var totalCount = await queryable.CountAsync();

        var units = await queryable
            .OrderBy(b => b.Name)
            .Skip((unitQuery.Page - 1) * unitQuery.PageSize)
            .Take(unitQuery.PageSize)
            .Select(x => new GetUnitResponse(x.Id, x.Name, x.ShortCode, x.ConversionToBase))
            .ToListAsync();

        var response = new Paging<GetUnitResponse>()
        {
            Data = units,
            Page = unitQuery.Page,
            PageSize = unitQuery.PageSize,
            Total = totalCount
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    async Task<ActionResult> Get(string id)
    {
        var find = await context.Units.FindAsync(id);
        if (find is null)
            return NotFound();
        var response = new GetUnitResponse(find.Id, find.Name, find.ShortCode, find.ConversionToBase);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUnitRequest request)
    {
        var unit = new Unit()
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            ShortCode = request.ShortCode,
            ConversionToBase = request.ConversionToBase
        };
        await context.Units.AddAsync(unit);
        await context.SaveChangesAsync();
        var response = new GetUnitResponse(unit.Id, unit.Name, unit.ShortCode, unit.ConversionToBase);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateUnitRequest request)
    {
        var find = await context.Units.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        find.ShortCode = request.ShortCode;
        find.ConversionToBase = request.ConversionToBase;
        await context.SaveChangesAsync();
        var response = new GetUnitResponse(find.Id, find.Name, find.ShortCode, find.ConversionToBase);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Units.FindAsync(id);
        if (find is null)
            return NotFound();

        context.Units.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}