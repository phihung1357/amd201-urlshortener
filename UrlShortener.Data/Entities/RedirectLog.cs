using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Data.Entities;

public class RedirectLog
{
    public Guid Id { get; set; }

    public Guid ShortUrlId { get; set; }

    public DateTimeOffset AccessedAt { get; set; } = DateTimeOffset.UtcNow;

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public string? Referrer { get; set; }

    public ShortUrl? ShortUrl { get; set; }
}
