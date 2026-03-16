using System;
using System.Collections.Generic;
using System.Text;

using UrlShortener.Common.Enums;

namespace UrlShortener.Common.DTOs;

public class ShortUrlResponseDto
{
    public Guid Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public string? CustomAlias { get; set; }
    public string ShortLink { get; set; } = string.Empty;
    public ShortUrlStatusEnum Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public DateTimeOffset? LastAccessedAt { get; set; }
    public long TotalClicks { get; set; }
}
