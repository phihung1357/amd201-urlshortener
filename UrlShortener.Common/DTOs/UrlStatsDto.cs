using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Common.DTOs;

public class UrlStatsDto
{
    public Guid UrlId { get; set; }
    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortCode { get; set; } = string.Empty;
    public long TotalClicks { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastAccessedAt { get; set; }
}