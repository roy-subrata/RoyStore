using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseReturnController(
    ILogger<PurchaseReturnController> logger,
    StoreDbContext dbContext
    ):ControllerBase
{
    
    
}