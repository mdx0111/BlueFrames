using BlueFrames.Persistence.DataContext;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BlueFrames.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog(config =>
        {
            config
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext();
        });

        services
            .AddHealthChecks()
            .AddDbContextCheck<AppDbContext>();

        services.AddOpenApi(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Contact = new OpenApiContact
                {
                    Name = "BlueFrames Support",
                    Email = "support@blue-frames.com"
                };
                return Task.CompletedTask;
            });
        });
        
        services.AddHttpContextAccessor();

        services.AddRouting();
        
        services.AddControllers();

        return services;
    }
}
