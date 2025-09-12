using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

// ---------- DTOs ----------
public class CategoryDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ParentId { get; set; }
    public List<CategoryDto> Children { get; set; } = new();
}

public record CreateCategoryRequest(string Name, string Description, string? ParentId);
public record UpdateCategoryRequest(string Name, string Description,string? ParentId);
public record GetCategoryResponse(string Id, string Name, string? Description);

// ---------- Helper Models ----------


// ---------- Extensions ----------
public static class CategoryMapper
{
    public static CategoryDto MapToDto(Category category) =>
        new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            ParentId = category.ParentId,
            Children = category.Children.Select(MapToDto).ToList()
        };
}

// ---------- Controller ----------
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ILogger<CategoriesController> logger,
    StoreDbContext context)
    : ControllerBase
{
    // GET: api/categories?search=abc&page=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query query)
    {
        logger.LogInformation("Fetching categories with request: {@Request}", query);

        var categoriesQuery = context.Categories
            .Include(c => c.Children)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            categoriesQuery = categoriesQuery
                .Where(c => c.Name.ToLower().Contains(query.Search.ToLower()));
        }

        var totalCount = await categoriesQuery.CountAsync();

        var categories = await categoriesQuery
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.Name)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var result = new Paging<CategoryDto>
        {
            Data = categories.Select(CategoryMapper.MapToDto).ToList(),
            Page = query.Page,
            PageSize = query.PageSize,
            Total = totalCount
        };

        return Ok(result);
    }

    // GET: api/categories/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<GetCategoryResponse>> GetById(string id)
    {
        var find = await context.Categories.FindAsync(id);
        if (find is null)
            return NotFound();

        var response = new GetCategoryResponse(find.Id, find.Name, find.Description);
        return Ok(response);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCategoryRequest request)
    {
        var category = new Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId
        };

        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var response = new GetCategoryResponse(category.Id, category.Name, category.Description);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, response);
    }

    // PUT: api/categories/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCategoryRequest request)
    {
        var find = await context.Categories.FindAsync(id);
        if (find is null)
        {
            return NotFound();
        }

        find.Name = request.Name;
        find.Description = request.Description;

        await context.SaveChangesAsync();

        var response = new GetCategoryResponse(find.Id, find.Name, find.Description);
        return Ok(response);
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Categories
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (find is null)
            return NotFound();

        if (find.Children.Any())
            return BadRequest("Cannot delete a category that has children.");

        context.Categories.Remove(find);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
