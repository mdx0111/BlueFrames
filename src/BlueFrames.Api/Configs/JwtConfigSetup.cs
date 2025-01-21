using Microsoft.Extensions.Options;

namespace BlueFrames.Api.Configs;

public class JwtConfigSetup: IConfigureOptions<JwtConfig>
{
    private const string SectionName = "JwtConfig";
    private readonly IConfiguration _configuration;

    public JwtConfigSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(JwtConfig options)
    {
        _configuration
            .GetSection(SectionName)
            .Bind(options);
    }
}