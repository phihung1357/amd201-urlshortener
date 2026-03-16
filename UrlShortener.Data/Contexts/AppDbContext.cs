using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Configurations;
using UrlShortener.Data.Entities;

namespace UrlShortener.Data.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();
    public DbSet<RedirectLog> RedirectLogs => Set<RedirectLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ShortUrlConfiguration());
        modelBuilder.ApplyConfiguration(new RedirectLogConfiguration());
    }
}
