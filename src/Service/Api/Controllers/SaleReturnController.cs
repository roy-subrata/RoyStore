using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleReturnController(
    ILogger<SaleReturnController> logger,
    StoreDbContext dbContext,
    IMapper mapper
) : ControllerBase
{
}