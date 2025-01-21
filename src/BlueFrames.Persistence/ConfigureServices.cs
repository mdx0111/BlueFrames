using BlueFrames.Application.Interfaces.Common;
using BlueFrames.Persistence.Common.Database;
using BlueFrames.Persistence.Common.Extensions;
using BlueFrames.Persistence.Common.Services;
using BlueFrames.Persistence.DataContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlueFrames.Persistence;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeService, DateTimeService>();

        services
            .AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("BlueFramesDbConnection"));
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
            });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.Scan(scan =>
        {
            scan
                .FromAssemblyOf<IMarker>()
                .AddClasses(filter =>
                {
                    filter.Where(type => type.Name.EndsWith("Repository"));
                })
                .AsMatchingInterface()
                .WithScopedLifetime();
        });

        return services;
    }
}