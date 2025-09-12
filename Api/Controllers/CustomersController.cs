using Api.Entities;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(
    ILogger<CustomersController> logger,
    StoreDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Query customerQuery)
    {
        logger.LogInformation("Get all ");

        var customers = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(customerQuery.Search))
        {
            var searchLower = customerQuery.Search.ToLower();

            customers = customers.Where(x =>
                x.Name.ToLower().Contains(searchLower) ||
                x.Email.ToLower().Contains(searchLower) ||
                x.Phone.ToLower().Contains(searchLower));
        }

        var total = await customers.CountAsync();

        var resullt = await customers
            .OrderBy(x => x.Name)
            .Skip((customerQuery.Page - 1) * customerQuery.PageSize)
            .Take(customerQuery.PageSize)
            .Select(x => new GetCustomerResponse(x.Id, x.Name, x.Phone, x.Email, x.Address))
            .ToListAsync();

        var result = new Paging<GetCustomerResponse>
        {
            Data = resullt,
            Total = total,
            Page = customerQuery.Page,
            PageSize = customerQuery.PageSize
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var find = await context.Customers.FindAsync(id);
        if (find is null)
            return NotFound();
        var response = new GetCustomerResponse(find.Id, find.Name, find.Phone, find.Email, find.Address);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCustomerRequest request)
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
        var response = new GetCustomerResponse(customer.Id, customer.Name, customer.Email, customer.Phone, customer.Address);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] UpdateCustomerRequest request)
    {
        var find = await context.Customers.FindAsync(id);
        if (find is null)
            return NotFound();

        find.Name = request.Name;
        find.Phone = request.Phone;
        find.Email = request.Email;
        find.Address = request.Address;
        await context.SaveChangesAsync();
        var response = new GetCustomerResponse(find.Id, find.Name, find.Phone, find.Email, find.Address);
        return Ok(response);
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