// using Api.Mapping;
// using Api.Models;
// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;

// namespace Api.Controllers;

// [ApiController]
// [Route("api/[controller]")]
// public class StockController(
//     ILogger<StockController> logger,
//     IMapper mapper,
//     StoreDbContext context) : ControllerBase
// {
//     [HttpGet]
//     public async Task<IActionResult> Get([FromQuery] BrandQuery request)
//     {       
//         logger
//             .LogInformation("Get stock query with search: {search} page {page} pageSize {pageSize}", request.Search, request.Page, request.PageSize);
       
//         var query = context.PurchaseItems.AsNoTracking();

//         if (!string.IsNullOrEmpty(request.Search))
//         {
//             query = query.Where(c =>
//                 c.Product.Name.ToLower().Contains(request.Search.ToLower())
//                 || c.Product.LocalName.ToLower().Contains(request.Search.ToLower())
//                 || (c.Product.PartNo ?? string.Empty).ToLower().Contains(request.Search.ToLower())
//                 || c.Product.Category.Name.ToLower().Contains(request.Search.ToLower())
//             );
//         }

//         var numberOfRecord = await query.CountAsync();

//         var result = await query
//             .Skip((request.Page - 1) * request.PageSize)
//             .Take(request.PageSize)
//             .Include(x => x.Purchase)
//             .ThenInclude(x => x.Supplier)
//             .Include(x => x.Product)
//             .ThenInclude(x => x.Attributes)
//             .Include(x => x.Product)
//             .ThenInclude(x => x.Category)
//             .Include(x => x.Product)
//             .ThenInclude(x => x.Brand)
//             .Select(x => x.AsDto())
//             .ToListAsync();
    
//         var pageResult = new Paging<GetStock>()
//         {
//             Data = result,
//             Page = request.Page,
//             PageSize = request.PageSize,
//             Total = numberOfRecord
//         };

//         logger.LogInformation("Get stock result count:{total}",numberOfRecord);
//         return Ok(pageResult);
//     }
// }