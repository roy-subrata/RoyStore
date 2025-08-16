using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(
    ILogger<CustomerController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CustomerQuery request)
    {
        var query = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(searchLower) ||
                x.Phone.ToLower().Contains(searchLower));
        }

        var total = await query.CountAsync();

        var customers = await query
            .OrderBy(x => x.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetCustomer(x.Id, x.Name,x.Email, x.Phone, x.Address))
            .ToListAsync();

        var result = new Paging<GetCustomer>
        {
            Data = customers,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var find = await context.Customers.FindAsync(id);
        if (find is null)
            return NotFound();
        return Ok(new GetCustomer(find.Id, find.Name, find.Email,find.Phone, find.Address));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCustomer request)
    {
        var customer = new Customer()
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,  
            Address = request.Address
        };
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
        return Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCustomer request)
    {
        var find = await context.Customers.FindAsync(id);
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
        var find = await context.Customers.FindAsync(id);
        if (find is null)
            return NotFound();
        context.Customers.Remove(find);
        await context.SaveChangesAsync();
        return Ok();
    }
}