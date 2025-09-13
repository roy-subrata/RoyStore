using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnitConversionController(
    ILogger<UnitsController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query unitQuery)
    {
        logger.LogInformation("Get Unit Conversion");
        var queryable = context.UnitConversions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(unitQuery.Search))
        {
            queryable = queryable.Where(c => c.FromUnit.Name.ToLower().Contains(unitQuery.Search.ToLower()));
        }

        var totalCount = await queryable.CountAsync();

        var unitConversions = await queryable
            .Skip((unitQuery.Page - 1) * unitQuery.PageSize)
            .Take(unitQuery.PageSize)
            .Select(x => new GetUnitConversionResponse(
                x.Id, new EntityRef(x.FromUnit.Id, x.FromUnit.Name),
                new EntityRef(x.FromUnit.Id, x.FromUnit.Name), x.Factor))
            .ToListAsync();

        var response = new Paging<GetUnitConversionResponse>()
        {
            Data = unitConversions,
            Page = unitQuery.Page,
            PageSize = unitQuery.PageSize,
            Total = totalCount
        };

        return Ok(response);
    }

    [HttpGet("{id}")]
    async Task<ActionResult> Get(string id)
    {
        var find = await context.UnitConversions.FindAsync(id);
        if (find is null)
            return NotFound();
        var response = new GetUnitConversionResponse(
               find.Id, new EntityRef(find.FromUnit.Id, find.FromUnit.Name),
                new EntityRef(find.FromUnit.Id, find.FromUnit.Name), find.Factor);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateUnitConversionRequest request)
    {

        var formUnit = await context.Units.FindAsync(request.FromUnitId);
        if (formUnit is null)
        {
            return BadRequest("From unit not found.");
        }
        var toUnit = await context.Units.FindAsync(request.ToUnitId);
        if (toUnit is null)
        {
            return BadRequest("To unit not found.");
        }

        var unitConversion = new UnitConversion()
        {
            Id = Guid.NewGuid().ToString(),
            FromUnitId = request.FromUnitId,
            ToUnitId = request.FromUnitId,
            Factor = request.Factor
        };
        await context.UnitConversions.AddAsync(unitConversion);
        await context.SaveChangesAsync();
        return Ok(unitConversion.Id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateUnitConversionRequest request)
    {
        var find = await context.UnitConversions.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }
        var formUnit = await context.Units.FindAsync(request.FromUnitId);
        if (formUnit is null)
        {
            return BadRequest("From unit not found.");
        }
        var toUnit = await context.Units.FindAsync(request.ToUnitId);
        if (toUnit is null)
        {
            return BadRequest("To unit not found.");
        }

        find.FromUnitId = request.FromUnitId;
        find.ToUnitId = request.FromUnitId;
        find.Factor = request.Factor;
        await context.SaveChangesAsync();
        var response = new GetUnitConversionResponse(
                find.Id, new EntityRef(find.FromUnit.Id, find.FromUnit.Name),
                 new EntityRef(find.FromUnit.Id, find.FromUnit.Name), find.Factor);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.UnitConversions.FindAsync(id);
        if (find is null)
            return NotFound();

        context.UnitConversions.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}