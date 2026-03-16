using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Common.Constants;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data.Configurations;

public class ShortUrlConfiguration : IEntityTypeConfiguration<ShortUrl>
{
    public void Configure(EntityTypeBuilder<ShortUrl> builder)
    {
        builder.ToTable("ShortUrls");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalUrl)
            .IsRequired()
            .HasMaxLength(MaxLengths.OriginalUrl);

        builder.Property(x => x.ShortCode)
            .IsRequired()
            .HasMaxLength(MaxLengths.ShortCode);

        builder.Property(x => x.CustomAlias)
            .HasMaxLength(MaxLengths.ShortCode);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => x.ShortCode)
            .IsUnique();

        builder.HasMany(x => x.RedirectLogs)
            .WithOne(x => x.ShortUrl)
            .HasForeignKey(x => x.ShortUrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
