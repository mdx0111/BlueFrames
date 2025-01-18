namespace BlueFrames.Api.Tests.Integration;

public class UnitTest1 : IClassFixture<BlueFramesApiFactory>
{
    private readonly HttpClient _httpClient;

    public UnitTest1(BlueFramesApiFactory factory)
    {
        _httpClient = factory.CreateClient();
    }
}
