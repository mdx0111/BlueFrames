namespace BlueFrames.Api.V1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiController : ControllerBase;
