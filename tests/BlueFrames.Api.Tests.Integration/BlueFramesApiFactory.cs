using BlueFrames.Api.Configs;
using BlueFrames.Persistence.DataContext;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Testcontainers.MsSql;

namespace BlueFrames.Api.Tests.Integration;

public class BlueFramesApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("SQL Server is now ready for client connections."))
        .WithPassword("Password123!")
        .Build();

    private IConfiguration Configuration { get; set; }
    private JwtConfig JwtConfig { get; set; }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        builder.ConfigureAppConfiguration((_, conf) =>
        {
            conf.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.Integration.json"));
            conf.AddEnvironmentVariables();
            Configuration = conf.Build();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_msSqlContainer.GetConnectionString()),
                ServiceLifetime.Scoped,
                ServiceLifetime.Singleton);

            JwtConfig = Configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();
        });
    }

    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public HttpClient CreateHttpClientWithAdminCredentials()
    {
        return CreateClient().WithUserCredentials("admin", JwtConfig); 
    }

    public HttpClient CreateHttpClientWithUserCredentials()
    {
        return CreateClient().WithUserCredentials("user", JwtConfig); 
    }

    public new async Task DisposeAsync()
    {
        await _msSqlContainer.StopAsync();
        await _msSqlContainer.DisposeAsync();
    }
}

