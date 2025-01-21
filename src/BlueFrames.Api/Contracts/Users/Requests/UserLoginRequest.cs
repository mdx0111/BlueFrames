namespace BlueFrames.Api.Contracts.Users.Requests;

public record UserLoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string[] Roles { get; set; }
}