using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data.Configurations;

public class RedirectLogConfiguration : IEntityTypeConfiguration<RedirectLog>
{
    public void Configure(EntityTypeBuilder<RedirectLog> builder)
    {
        builder.ToTable("RedirectLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.AccessedAt)
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasMaxLength(100);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.Referrer)
            .HasMaxLength(500);
    }
}
