using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using UrlShortener.Common.DTOs;
using Xunit;

namespace UrlShortener.IntegrationTests;

public class UrlsApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UrlsApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Post_CreateShortUrl_ShouldReturnCreated()
    {
        await _factory.ResetDatabaseAsync();

        var alias = $"alias-{Guid.NewGuid():N}".Substring(0, 12);

        var request = new
        {
            originalUrl = "https://learn.microsoft.com",
            expiresAt = (string?)null,
            customAlias = alias
        };

        var response = await _client.PostAsJsonAsync("/api/Urls", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<ShortUrlResponseDto>();
        result.Should().NotBeNull();
        result!.OriginalUrl.Should().Be("https://learn.microsoft.com");
        result.ShortCode.Should().Be(alias);
        result.CustomAlias.Should().Be(alias);
    }

    [Fact]
    public async Task GetAll_AfterCreate_ShouldContainCreatedItem()
    {
        await _factory.ResetDatabaseAsync();

        var alias = $"list-{Guid.NewGuid():N}".Substring(0, 11);

        await _client.PostAsJsonAsync("/api/Urls", new
        {
            originalUrl = "https://github.com",
            expiresAt = (string?)null,
            customAlias = alias
        });

        var response = await _client.GetAsync("/api/Urls");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var items = await response.Content.ReadFromJsonAsync<List<ShortUrlResponseDto>>();
        items.Should().NotBeNull();
        items!.Any(x => x.ShortCode == alias).Should().BeTrue();
    }

    [Fact]
    public async Task Redirect_WithValidCode_ShouldReturnRedirectResponse()
    {
        await _factory.ResetDatabaseAsync();

        var alias = $"go-{Guid.NewGuid():N}".Substring(0, 10);
        var originalUrl = "https://learn.microsoft.com";

        await _client.PostAsJsonAsync("/api/Urls", new
        {
            originalUrl,
            expiresAt = (string?)null,
            customAlias = alias
        });

        var response = await _client.GetAsync($"/r/{alias}");

        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location.Should().Be(new Uri(originalUrl));
    }

    [Fact]
    public async Task Delete_AfterCreate_ShouldMakeRedirectReturnNotFound()
    {
        await _factory.ResetDatabaseAsync();

        var alias = $"del-{Guid.NewGuid():N}".Substring(0, 11);

        var createResponse = await _client.PostAsJsonAsync("/api/Urls", new
        {
            originalUrl = "https://learn.microsoft.com",
            expiresAt = (string?)null,
            customAlias = alias
        });

        var created = await createResponse.Content.ReadFromJsonAsync<ShortUrlResponseDto>();
        created.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/api/Urls/{created!.Id}");
        var redirectResponse = await _client.GetAsync($"/r/{alias}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        redirectResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}