using System;
using System.Collections.Generic;
using System.Text;

using UrlShortener.Common.Enums;

namespace UrlShortener.Data.Entities;

public class ShortUrl
{
    public Guid Id { get; set; }

    public string OriginalUrl { get; set; } = string.Empty;

    public string ShortCode { get; set; } = string.Empty;

    public string? CustomAlias { get; set; }

    public ShortUrlStatusEnum Status { get; set; } = ShortUrlStatusEnum.Active;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresAt { get; set; }

    public DateTimeOffset? LastAccessedAt { get; set; }

    public long TotalClicks { get; set; }

    public ICollection<RedirectLog> RedirectLogs { get; set; } = new List<RedirectLog>();
}
