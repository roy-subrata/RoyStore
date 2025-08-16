using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitController(
    ILogger<UnitController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] UnitQuery request)
    {
        logger.LogInformation("Get Unit");
        var query = context.Units.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(c => c.Name.ToLower().Contains(request.Search.ToLower()));
        }

        var totalCount = await query.CountAsync();

        var units = await query
            .OrderBy(b => b.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetUnit(x.Id, x.Name, x.Symbol))
            .ToListAsync();

        var pageResult = new Paging<GetUnit>()
        {
            Data = units,
            Page = request.Page,
            PageSize = request.PageSize,
            Total = totalCount
        };
        return Ok(pageResult);
    }

    [HttpGet]
    async Task<ActionResult> GetById(string id)
    {
        var find = await context.Units.FindAsync(id);
        if (find is null)
            return NotFound();
        return  new CreatedAtRouteResult("GetById", new { id = id }, find);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUnit request)
    {
        var unit = new Unit() { Id = Guid.NewGuid().ToString(), Name = request.Name, Symbol = request.Symbol };
        await context.Units.AddAsync(unit);
        await context.SaveChangesAsync();
        return Ok(unit);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateUnit request)
    {
        var find = await context.Units.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        find.Symbol = request.Symbol;
        await context.SaveChangesAsync();
        return Ok();
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