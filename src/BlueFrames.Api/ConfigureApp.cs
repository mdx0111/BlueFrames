using BlueFrames.Persistence.DataContext;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BlueFrames.Api;

public static class ConfigureApp
{
    public static void ConfigureHost(this WebApplication app, IConfiguration configuration)
    {
        app.UseRouting();

        app.MapControllers();
        
        if (app.Environment.IsDevelopment())
        {
             app.MapOpenApi("/openapi/v1.json");
        }

        app.UseSerilogRequestLogging();

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = (check) => check.Tags.Contains("ready")
        });
    }

    public static void ApplyDatabaseMigrations(this WebApplication app)
    {
        ILogger<Program> logger = null;
        using var scope = app.Services.CreateScope();

        try
        {
            logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            appDbContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to run migrations on database");
        }
    }
}