
using Api.Entities.Experiment;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeaturesController(
    ILogger<FeaturesController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query featuresQuery)
    {
        logger.LogInformation("Fetching Features result");
        var queryable = context.Features.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(featuresQuery.Search))
        {
            queryable = queryable.Where(b => b.Name.ToLower().Contains(featuresQuery.Search.ToLower()));
        }
        var totalCount = await queryable.CountAsync();

        var featuress = await queryable
            .OrderBy(b => b.Name)
            .Skip((featuresQuery.Page - 1) * featuresQuery.PageSize)
            .Take(featuresQuery.PageSize)
            .Select(x => new GetFeatureResponse(x.Id, x.Name, x.IsActive))
            .ToListAsync();

        var response = new Paging<GetFeatureResponse>()
        {
            Data = featuress,
            Page = featuresQuery.Page,
            PageSize = featuresQuery.PageSize,
            Total = totalCount
        };
        return Ok(response);
    }

    [HttpGet("{id}")]
  public  async Task<ActionResult> Get(string id)
    {
        var find = await context.Features.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetFeatureResponse(find.Id, find.Name, find.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateFeaturesRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var features = new Feature { Id = Guid.NewGuid().ToString(), Name = request.Name, IsActive = request.IsActive };
        await context.Features.AddAsync(features);
        await context.SaveChangesAsync();
        return Ok(new GetFeatureResponse(features.Id, features.Name, features.IsActive));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateFeaturesRequest request)
    {

        var find = await context.Features.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        find.IsActive = request.IsActive;
        await context.SaveChangesAsync();
        return Ok(new GetFeatureResponse(find.Id, find.Name, find.IsActive));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Features.FindAsync(id);
        if (find is null)
            return NotFound();
        context.Features.Remove(find);
        await context.SaveChangesAsync();
        return NoContent();
    }
}

public record CreateFeaturesRequest(string Name, bool IsActive);
public record UpdateFeaturesRequest(string Id, string Name, bool IsActive);
public record GetFeatureResponse(string Id, string Name, bool IsActive);