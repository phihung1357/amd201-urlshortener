using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.DTOs;
using UrlShortener.Common.Enums;
using UrlShortener.Data.Contexts;
using UrlShortener.Data.Entities;
using UrlShortener.Services.Interfaces;

namespace UrlShortener.Services.Implementations;

public class ShortUrlService : IShortUrlService
{
    private readonly AppDbContext _dbContext;

    public ShortUrlService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ShortUrl> CreateAsync(CreateShortUrlRequestDto request)
    {
        ValidateOriginalUrl(request.OriginalUrl);

        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("ExpiresAt must be in the future.");
        }

        string shortCode;

        if (!string.IsNullOrWhiteSpace(request.CustomAlias))
        {
            shortCode = request.CustomAlias.Trim();

            var aliasExists = await _dbContext.ShortUrls
                .AnyAsync(x => x.ShortCode == shortCode);

            if (aliasExists)
            {
                throw new ArgumentException("CustomAlias already exists.");
            }
        }
        else
        {
            shortCode = await GenerateUniqueCodeAsync();
        }

        var entity = new ShortUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = request.OriginalUrl.Trim(),
            ShortCode = shortCode,
            CustomAlias = string.IsNullOrWhiteSpace(request.CustomAlias) ? null : request.CustomAlias.Trim(),
            Status = ShortUrlStatusEnum.Active,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = request.ExpiresAt,
            TotalClicks = 0
        };

        _dbContext.ShortUrls.Add(entity);
        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<IReadOnlyList<ShortUrl>> GetAllAsync()
    {
        return await _dbContext.ShortUrls
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<ShortUrl?> GetByIdAsync(Guid id)
    {
        return await _dbContext.ShortUrls
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ShortUrl?> ResolveAsync(string code, string? ipAddress, string? userAgent, string? referrer)
    {
        var entity = await _dbContext.ShortUrls
            .Include(x => x.RedirectLogs)
            .FirstOrDefaultAsync(x => x.ShortCode == code);

        if (entity is null)
        {
            return null;
        }

        if (entity.Status != ShortUrlStatusEnum.Active)
        {
            return null;
        }

        if (entity.ExpiresAt.HasValue && entity.ExpiresAt.Value <= DateTimeOffset.UtcNow)
        {
            entity.Status = ShortUrlStatusEnum.Expired;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        entity.LastAccessedAt = DateTimeOffset.UtcNow;
        entity.TotalClicks += 1;

        _dbContext.RedirectLogs.Add(new RedirectLog
        {
            Id = Guid.NewGuid(),
            ShortUrlId = entity.Id,
            AccessedAt = DateTimeOffset.UtcNow,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Referrer = referrer
        });

        await _dbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<ShortUrl?> UpdateAsync(Guid id, UpdateShortUrlRequestDto request)
    {
        var entity = await _dbContext.ShortUrls.FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            ValidateOriginalUrl(request.OriginalUrl);
            entity.OriginalUrl = request.OriginalUrl.Trim();
        }

        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("ExpiresAt must be in the future.");
        }

        entity.ExpiresAt = request.ExpiresAt;

        if (!string.IsNullOrWhiteSpace(request.CustomAlias))
        {
            var newAlias = request.CustomAlias.Trim();

            var aliasExists = await _dbContext.ShortUrls
                .AnyAsync(x => x.ShortCode == newAlias && x.Id != id);

            if (aliasExists)
            {
                throw new ArgumentException("CustomAlias already exists.");
            }

            entity.CustomAlias = newAlias;
            entity.ShortCode = newAlias;
        }

        if (entity.Status == ShortUrlStatusEnum.Inactive)
        {
            entity.Status = ShortUrlStatusEnum.Active;
        }

        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _dbContext.ShortUrls.FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return false;
        }

        entity.Status = ShortUrlStatusEnum.Inactive;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<UrlStatsDto?> GetStatsAsync(Guid id)
    {
        var entity = await _dbContext.ShortUrls
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity is null)
        {
            return null;
        }

        return new UrlStatsDto
        {
            UrlId = entity.Id,
            OriginalUrl = entity.OriginalUrl,
            ShortCode = entity.ShortCode,
            TotalClicks = entity.TotalClicks,
            CreatedAt = entity.CreatedAt,
            LastAccessedAt = entity.LastAccessedAt
        };
    }

    private static void ValidateOriginalUrl(string originalUrl)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
        {
            throw new ArgumentException("OriginalUrl is required.");
        }

        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("OriginalUrl must be a valid HTTP/HTTPS URL.");
        }
    }

    private async Task<string> GenerateUniqueCodeAsync(int length = 7)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        while (true)
        {
            var chars = new char[length];

            for (int i = 0; i < length; i++)
            {
                chars[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
            }

            var code = new string(chars);

            var exists = await _dbContext.ShortUrls.AnyAsync(x => x.ShortCode == code);
            if (!exists)
            {
                return code;
            }
        }
    }
}
