using Api;
using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UnitConversionController(
    ILogger<UnitConversionController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Paging<GetUnitConversionResponse>>> Get([FromQuery] Query unitQuery)
    {
        logger.LogInformation("Get Unit Conversion");

        var queryable = context.UnitConversions
            .Include(x => x.FromUnit)
            .Include(x => x.ToUnit)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(unitQuery.Search))
        {
            queryable = queryable.Where(c => c.FromUnit.Name.ToLower().Contains(unitQuery.Search.ToLower()));
        }

        var totalCount = await queryable.CountAsync();

        var unitConversions = await queryable
            .Skip((unitQuery.Page - 1) * unitQuery.PageSize)
            .Take(unitQuery.PageSize)
            .Select(x => new GetUnitConversionResponse(
                x.Id,
                new EntityRef(x.FromUnit.Id, x.FromUnit.Name),
                new EntityRef(x.ToUnit.Id, x.ToUnit.Name),
                x.Factor))
            .ToListAsync();

        return Ok(new Paging<GetUnitConversionResponse>
        {
            Data = unitConversions,
            Page = unitQuery.Page,
            PageSize = unitQuery.PageSize,
            Total = totalCount
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetUnitConversionResponse>> GetById(string id)
    {
        var find = await context.UnitConversions
            .Include(x => x.FromUnit)
            .Include(x => x.ToUnit)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (find is null) return NotFound();

        return Ok(new GetUnitConversionResponse(
            find.Id,
            new EntityRef(find.FromUnit.Id, find.FromUnit.Name),
            new EntityRef(find.ToUnit.Id, find.ToUnit.Name),
            find.Factor));
    }

    [HttpPost]
    public async Task<ActionResult<GetUnitConversionResponse>> Post([FromBody] CreateUnitConversionRequest request)
    {
        var fromUnit = await context.Units.FindAsync(request.FromUnitId);
        if (fromUnit is null) return BadRequest("From unit not found.");

        var toUnit = await context.Units.FindAsync(request.ToUnitId);
        if (toUnit is null) return BadRequest("To unit not found.");

        var unitConversion = new UnitConversion
        {
            Id = Guid.NewGuid().ToString(),
            FromUnitId = request.FromUnitId,
            ToUnitId = request.ToUnitId,
            Factor = request.Factor
        };

        await context.UnitConversions.AddAsync(unitConversion);
        await context.SaveChangesAsync();

        var response = new GetUnitConversionResponse(
            unitConversion.Id,
            new EntityRef(fromUnit.Id, fromUnit.Name),
            new EntityRef(toUnit.Id, toUnit.Name),
            unitConversion.Factor);

        return CreatedAtAction(nameof(GetById), new { id = unitConversion.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GetUnitConversionResponse>> Put(string id, [FromBody] UpdateUnitConversionRequest request)
    {
        var find = await context.UnitConversions.FindAsync(id);
        if (find is null) return NotFound();

        var fromUnit = await context.Units.FindAsync(request.FromUnitId);
        if (fromUnit is null) return BadRequest("From unit not found.");

        var toUnit = await context.Units.FindAsync(request.ToUnitId);
        if (toUnit is null) return BadRequest("To unit not found.");

        find.FromUnitId = request.FromUnitId;
        find.ToUnitId = request.ToUnitId;
        find.Factor = request.Factor;

        await context.SaveChangesAsync();

        return Ok(new GetUnitConversionResponse(
            find.Id,
            new EntityRef(fromUnit.Id, fromUnit.Name),
            new EntityRef(toUnit.Id, toUnit.Name),
            find.Factor));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.UnitConversions.FindAsync(id);
        if (find is null) return NotFound();

        context.UnitConversions.Remove(find);
        await context.SaveChangesAsync();

        return NoContent(); // 204 is better for delete
    }
}
