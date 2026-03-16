using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Common.DTOs;
using UrlShortener.Common.Enums;
using UrlShortener.Data.Contexts;
using UrlShortener.Data.Entities;
using UrlShortener.Services.Implementations;
using Xunit;

namespace UrlShortener.UnitTests;

public class ShortUrlServiceTests
{
    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateAsync_WithValidUrl_ShouldCreateShortUrl()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var service = new ShortUrlService(dbContext);

        var request = new CreateShortUrlRequestDto
        {
            OriginalUrl = "https://learn.microsoft.com",
            CustomAlias = "learn-ms"
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.OriginalUrl.Should().Be("https://learn.microsoft.com");
        result.ShortCode.Should().Be("learn-ms");
        result.CustomAlias.Should().Be("learn-ms");
        result.Status.Should().Be(ShortUrlStatusEnum.Active);
        result.TotalClicks.Should().Be(0);

        var countInDb = await dbContext.ShortUrls.CountAsync();
        countInDb.Should().Be(1);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidUrl_ShouldThrowArgumentException()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var service = new ShortUrlService(dbContext);

        var request = new CreateShortUrlRequestDto
        {
            OriginalUrl = "abc-not-valid-url"
        };

        // Act
        Func<Task> act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*valid HTTP/HTTPS URL*");
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCustomAlias_ShouldThrowArgumentException()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var service = new ShortUrlService(dbContext);

        await service.CreateAsync(new CreateShortUrlRequestDto
        {
            OriginalUrl = "https://learn.microsoft.com",
            CustomAlias = "same-alias"
        });

        var secondRequest = new CreateShortUrlRequestDto
        {
            OriginalUrl = "https://github.com",
            CustomAlias = "same-alias"
        };

        // Act
        Func<Task> act = async () => await service.CreateAsync(secondRequest);

        // Assert
        await act.Should()
            .ThrowAsync<ArgumentException>()
            .WithMessage("*CustomAlias already exists*");
    }

    [Fact]
    public async Task ResolveAsync_WithValidCode_ShouldIncreaseClickCount_AndCreateRedirectLog()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var service = new ShortUrlService(dbContext);

        var created = await service.CreateAsync(new CreateShortUrlRequestDto
        {
            OriginalUrl = "https://learn.microsoft.com",
            CustomAlias = "resolve-me"
        });

        // Act
        var resolved = await service.ResolveAsync(
            "resolve-me",
            "127.0.0.1",
            "UnitTest-Agent",
            "http://localhost/test");

        // Assert
        resolved.Should().NotBeNull();
        resolved!.Id.Should().Be(created.Id);
        resolved.TotalClicks.Should().Be(1);
        resolved.LastAccessedAt.Should().NotBeNull();

        var entityInDb = await dbContext.ShortUrls.FirstAsync(x => x.Id == created.Id);
        entityInDb.TotalClicks.Should().Be(1);

        var logCount = await dbContext.RedirectLogs.CountAsync();
        logCount.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetStatusToInactive_AndResolveShouldReturnNull()
    {
        // Arrange
        await using var dbContext = CreateDbContext();
        var service = new ShortUrlService(dbContext);

        var created = await service.CreateAsync(new CreateShortUrlRequestDto
        {
            OriginalUrl = "https://learn.microsoft.com",
            CustomAlias = "delete-me"
        });

        // Act
        var deleted = await service.DeleteAsync(created.Id);
        var resolved = await service.ResolveAsync(
            "delete-me",
            "127.0.0.1",
            "UnitTest-Agent",
            "http://localhost/test");

        // Assert
        deleted.Should().BeTrue();
        resolved.Should().BeNull();

        var entityInDb = await dbContext.ShortUrls.FirstAsync(x => x.Id == created.Id);
        entityInDb.Status.Should().Be(ShortUrlStatusEnum.Inactive);
    }

    [Fact]
    public async Task ResolveAsync_WithExpiredUrl_ShouldReturnNull_AndMarkAsExpired()
    {
        // Arrange
        await using var dbContext = CreateDbContext();

        var entity = new ShortUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = "https://learn.microsoft.com",
            ShortCode = "expired-link",
            Status = ShortUrlStatusEnum.Active,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-1),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            TotalClicks = 0
        };

        dbContext.ShortUrls.Add(entity);
        await dbContext.SaveChangesAsync();

        var service = new ShortUrlService(dbContext);

        // Act
        var resolved = await service.ResolveAsync(
            "expired-link",
            "127.0.0.1",
            "UnitTest-Agent",
            "http://localhost/test");

        // Assert
        resolved.Should().BeNull();

        var entityInDb = await dbContext.ShortUrls.FirstAsync(x => x.Id == entity.Id);
        entityInDb.Status.Should().Be(ShortUrlStatusEnum.Expired);
    }
}