using BlueFrames.Api.Contracts.Users.Requests;

namespace BlueFrames.Api.Services;

public interface IJwtTokenService
{
    string GenerateToken(UserLoginRequest userLoginRequest);
}