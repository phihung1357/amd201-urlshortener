using Microsoft.AspNetCore.Mvc;
using UrlShortener.Services.Interfaces;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("r")]
public class RedirectController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;

    public RedirectController(IShortUrlService shortUrlService)
    {
        _shortUrlService = shortUrlService;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Resolve(string code)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();
        var referrer = Request.Headers.Referer.ToString();

        var entity = await _shortUrlService.ResolveAsync(code, ipAddress, userAgent, referrer);

        if (entity is null)
        {
            return NotFound(new { message = "Short URL not found, inactive, or expired." });
        }

        return Redirect(entity.OriginalUrl);
    }
}