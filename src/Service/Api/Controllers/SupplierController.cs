using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierController(
    ILogger<CustomerController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SupplierQuery request)
    {
        var query = context.Suppliers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{searchLower}%") ||
                EF.Functions.Like(x.Phone, $"%{searchLower}%"));
        }

        var total = await query.CountAsync();

        var suppliers = await query
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetSupplier(
                x.Id,
                x.Name,
                x.Email,
                x.Phone,
                x.Address
            )).ToListAsync();

        var result = new Paging<GetSupplier>
        {
            Data = suppliers,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var find = await context.Suppliers.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetSupplier(find.Id, find.Name, find.Email, find.Phone, find.Address));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSupplier request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var supplier = new Supplier()
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };
        await context.Suppliers.AddAsync(supplier);
        await context.SaveChangesAsync();
        return Ok(supplier);
        //return Created($"api/customers/{customer.Id}", customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateSupplier request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var find = await context.Suppliers.FindAsync(id);
        if (find is null)
            return NotFound();

        find.Name = request.Name;
        find.Phone = request.Phone;
        find.Email = request.Email;
        find.Address = request.Address;
        await context.SaveChangesAsync();
        return Ok(find);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var find = await context.Suppliers.FindAsync(id);
        if (find is null)
            return NotFound();
        context.Suppliers.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}