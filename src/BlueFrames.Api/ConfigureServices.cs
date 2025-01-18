using Asp.Versioning;
using BlueFrames.Persistence.DataContext;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BlueFrames.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiVersioningBuilder = services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });
        
        apiVersioningBuilder.AddApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        
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
            options.AddDocumentTransformer((document, _, _) =>
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
