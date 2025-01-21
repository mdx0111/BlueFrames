using BlueFrames.Api.Contracts.Users.Requests;
using BlueFrames.Api.Services;

namespace BlueFrames.Api.V1.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "v1")]
public class UserController : ApiController
{
    private readonly IJwtTokenService _jwtTokenService;

    public UserController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }
    
    [EndpointSummary("Logs user in by providing email and password")]
    [HttpPost]
    [Route("authenticate")]
    public ActionResult<string> Authenticate([FromBody] UserLoginRequest userLoginRequest)
    {
        var token = _jwtTokenService.GenerateToken(userLoginRequest);
        return Ok(token);
    }
}