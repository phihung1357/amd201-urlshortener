using Microsoft.AspNetCore.Mvc;
using UrlShortener.Common.DTOs;
using UrlShortener.Data.Entities;
using UrlShortener.Services.Interfaces;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;

    public UrlsController(IShortUrlService shortUrlService)
    {
        _shortUrlService = shortUrlService;
    }

    [HttpPost]
    public async Task<ActionResult<ShortUrlResponseDto>> Create([FromBody] CreateShortUrlRequestDto request)
    {
        try
        {
            var entity = await _shortUrlService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, MapToResponse(entity));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShortUrlResponseDto>>> GetAll()
    {
        var items = await _shortUrlService.GetAllAsync();
        return Ok(items.Select(MapToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ShortUrlResponseDto>> GetById(Guid id)
    {
        var entity = await _shortUrlService.GetByIdAsync(id);

        if (entity is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(entity));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ShortUrlResponseDto>> Update(Guid id, [FromBody] UpdateShortUrlRequestDto request)
    {
        try
        {
            var entity = await _shortUrlService.UpdateAsync(id, request);

            if (entity is null)
            {
                return NotFound();
            }

            return Ok(MapToResponse(entity));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _shortUrlService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id:guid}/stats")]
    public async Task<ActionResult<UrlStatsDto>> GetStats(Guid id)
    {
        var stats = await _shortUrlService.GetStatsAsync(id);

        if (stats is null)
        {
            return NotFound();
        }

        return Ok(stats);
    }

    private ShortUrlResponseDto MapToResponse(ShortUrl entity)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return new ShortUrlResponseDto
        {
            Id = entity.Id,
            OriginalUrl = entity.OriginalUrl,
            ShortCode = entity.ShortCode,
            CustomAlias = entity.CustomAlias,
            ShortLink = $"{baseUrl}/r/{entity.ShortCode}",
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            LastAccessedAt = entity.LastAccessedAt,
            TotalClicks = entity.TotalClicks
        };
    }
}
