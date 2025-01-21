using System.Net.Http.Headers;
using System.Security.Claims;
using BlueFrames.Api.Configs;

namespace BlueFrames.Api.Tests.Integration.Helpers;

public static class HttpClientExtensions
{
    public static HttpClient WithUserCredentials(this HttpClient client, string role, JwtConfig config)
    {
        if (string.IsNullOrEmpty(role))
        {
            return client;
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.Role, role)
        };
        var jwtToken = MockJwtTokens.GenerateJwtToken(claims, config);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        return client;
    }
}