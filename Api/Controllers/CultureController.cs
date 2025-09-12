// Web/Controllers/CultureController.cs
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CultureController : Controller
    {
        [HttpGet("Set")]
        public IActionResult Set(string culture, string redirectUri)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { IsEssential = true, Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }
            return LocalRedirect(redirectUri ?? "/");
        }
    }
}
