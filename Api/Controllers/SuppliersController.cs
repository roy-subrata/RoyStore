using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(
    ILogger<SuppliersController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query supplierQuery)
    {
        logger.LogInformation(
          "Querying suppliers with search: {Search}, page: {Page}, pageSize: {PageSize}",
          supplierQuery.Search, supplierQuery.Page, supplierQuery.PageSize
      );

        var query = context.Suppliers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(supplierQuery.Search))
        {
            var searchLower = supplierQuery.Search.ToLower();
            query = query.Where(x =>
                EF.Functions.Like(x.Name, $"%{searchLower}%") ||
                EF.Functions.Like(x.Phone, $"%{searchLower}%"));
        }

        var total = await query.CountAsync();

        var suppliers = await query
            .OrderBy(x => x.Name)
            .Skip((supplierQuery.Page - 1) * supplierQuery.PageSize)
            .Take(supplierQuery.PageSize)
            .Select(x => new GetSupplierResponse(
                x.Id,
                x.Name,
                x.Email,
                x.Phone,
                x.Address
            )).ToListAsync();

        var result = new Paging<GetSupplierResponse>
        {
            Data = suppliers,
            Total = total,
            Page = supplierQuery.Page,
            PageSize = supplierQuery.PageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var find = await context.Suppliers.FindAsync(id);
        if (find is null)
            return NotFound();
        var response = new GetSupplierResponse(find.Id, find.Name, find.Email, find.Phone, find.Address);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateSupplierRequest request)
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
        var response = new GetSupplierResponse(supplier.Id, supplier.Name, supplier.Email, supplier.Phone, supplier.Address);
        return Ok(response);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateSupplierRequest request)
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
        var response = new GetSupplierResponse(find.Id, find.Name, find.Email, find.Phone, find.Address);
        return Ok(response);
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