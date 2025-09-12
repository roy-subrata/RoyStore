using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleController(
    ILogger<SaleController> logger,
    StoreDbContext dbContext,
    IMapper mapper
) : ControllerBase
{
}