namespace BlueFrames.Api.Configs;

public record JwtConfig
{
    public string PrivateKey { get; init; }
    public string Issuer { get; init; }
    public string Audience { get; init; }
}