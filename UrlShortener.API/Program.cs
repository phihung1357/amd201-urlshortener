using Microsoft.EntityFrameworkCore;
using UrlShortener.Data.Contexts;
using UrlShortener.Services.Implementations;
using UrlShortener.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var useInMemoryDatabase =
    builder.Environment.IsEnvironment("Testing") ||
    builder.Configuration.GetValue<bool>("USE_INMEMORY_DATABASE");

if (useInMemoryDatabase)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("UrlShortenerDb"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddScoped<IShortUrlService, ShortUrlService>();

var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? new[]
    {
        "http://localhost:3000",
        "http://localhost:5173",
        "http://localhost:5174"
    };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment() && !app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

if (!useInMemoryDatabase && !app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
        .CreateLogger("StartupMigration");

    const int maxRetries = 20;

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            dbContext.Database.Migrate();
            logger.LogInformation("Database migration applied successfully.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Database migration failed on attempt {Attempt}/{MaxRetries}.", attempt, maxRetries);

            if (attempt == maxRetries)
            {
                throw;
            }

            Thread.Sleep(5000);
        }
    }
}

app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();

public partial class Program { }