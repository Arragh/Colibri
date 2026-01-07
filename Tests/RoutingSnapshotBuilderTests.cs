using Colibri.Configuration;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Tests;

public class RoutingSnapshotBuilderTests
{
    [Fact]
    public void Build_NoClusters_ThrowsNullReferenceException()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings();
        
        // Act + Assert
        Assert.Throws<NullReferenceException>(() => routingSnapshotBuilder.Build(settings));
    }
}