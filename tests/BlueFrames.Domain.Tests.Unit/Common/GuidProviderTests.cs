using BlueFrames.Domain.Common.CombGuid;

namespace BlueFrames.Domain.Tests.Unit.Common;

public class GuidProviderTests
{
    [Fact]
    public void Create_ShouldGenerateNewGuid()
    {
        // Act
        var guid = GuidProvider.Create();

        // Assert
        guid.Should().NotBeEmpty();
    }
    
    [Fact]
    public void Create_ShouldGenerateSequentialNewGuid()
    {
        // Act
        var firstGuid = GuidProvider.Create();
        var secondGuid = GuidProvider.Create();

        var firstGuidTimeStamp = GuidProvider.GetTimestamp(firstGuid);
        var secondGuidTimeStamp = GuidProvider.GetTimestamp(secondGuid);
        
        // Assert
        firstGuid.Should().NotBe(secondGuid);
        firstGuidTimeStamp.Ticks.Should().BeLessThanOrEqualTo(secondGuidTimeStamp.Ticks);
    }
}