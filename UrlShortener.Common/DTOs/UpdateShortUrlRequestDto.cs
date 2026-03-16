using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Common.DTOs;

public class UpdateShortUrlRequestDto
{
    public string? OriginalUrl { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
    public string? CustomAlias { get; set; }
}
