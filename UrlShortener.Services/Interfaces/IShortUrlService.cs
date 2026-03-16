using System;
using System.Collections.Generic;
using System.Text;

using UrlShortener.Common.DTOs;
using UrlShortener.Data.Entities;

namespace UrlShortener.Services.Interfaces;

public interface IShortUrlService
{
    Task<ShortUrl> CreateAsync(CreateShortUrlRequestDto request);
    Task<IReadOnlyList<ShortUrl>> GetAllAsync();
    Task<ShortUrl?> GetByIdAsync(Guid id);
    Task<ShortUrl?> ResolveAsync(string code, string? ipAddress, string? userAgent, string? referrer);
    Task<ShortUrl?> UpdateAsync(Guid id, UpdateShortUrlRequestDto request);
    Task<bool> DeleteAsync(Guid id);
    Task<UrlStatsDto?> GetStatsAsync(Guid id);
}
