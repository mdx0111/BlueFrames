using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlueFrames.Api.Configs;
using Microsoft.IdentityModel.Tokens;

namespace BlueFrames.Api.Tests.Integration.Helpers;

public static class MockJwtTokens
{
    public static string GenerateJwtToken(IEnumerable<Claim> claims, JwtConfig config)
    {
        var key = Encoding.ASCII.GetBytes(config.PrivateKey);
        var securityKey = new SymmetricSecurityKey(key);
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var handler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = config.Issuer,
            Audience = config.Audience,
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = signingCredentials
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}