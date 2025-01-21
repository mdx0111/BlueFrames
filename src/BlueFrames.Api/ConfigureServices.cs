using System.Security.Claims;
using System.Text;
using BlueFrames.Api.Configs;
using BlueFrames.Api.Services;
using BlueFrames.Persistence.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Serilog;

namespace BlueFrames.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtConfig = configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>();

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.PrivateKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ClockSkew = TimeSpan.Zero,
                    RequireSignedTokens = false
                };
            });
        
        services.ConfigureOptions<JwtConfigSetup>();
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        
        services
            .AddAuthorizationBuilder()
            .AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role,"admin"))
            .AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role,"user", "admin"));

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
            
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        
        services.AddHttpContextAccessor();

        services.AddRouting();
        
        services.AddControllers();

        return services;
    }
}
