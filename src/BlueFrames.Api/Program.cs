using BlueFrames.Application;
using BlueFrames.Persistence;

namespace BlueFrames.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var configuration = builder.Configuration;
        var services = builder.Services;
        
        // Add services to the container.
        services.AddApiServices(configuration);
        services.AddPersistenceServices(configuration);
        services.AddApplicationServices();

        var app = builder.Build();
        app.ConfigureHost(configuration);

        if (builder.Environment.IsProduction())
        {
            app.ApplyDatabaseMigrations();
        }

        app.Run();
    }
}
