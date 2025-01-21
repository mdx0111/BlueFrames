using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlueFrames.Api.Configs;
using BlueFrames.Api.Contracts.Users.Requests;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BlueFrames.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtConfig _jwtConfig;

    public JwtTokenService(IOptions<JwtConfig> authConfig)
    {
        _jwtConfig = authConfig.Value;
    }
    
    public string GenerateToken(UserLoginRequest userLoginRequest)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfig.PrivateKey);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(userLoginRequest),
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    private static ClaimsIdentity GenerateClaims(UserLoginRequest user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Name, user.Email));

        foreach (var role in user.Roles)
        {
            claims.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }
}