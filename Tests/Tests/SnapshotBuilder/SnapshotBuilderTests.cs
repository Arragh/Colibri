using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;
using Tests.Helpers;

namespace Tests.Tests.SnapshotBuilder;

public class SnapshotBuilderTests
{
    [Fact]
    public void Build_Result_NotNull()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotNull(routingSnapshot);
    }
    
    [Fact]
    public void Build_BasicFieldsAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.True(routingSnapshot.RootSegmentsCount > 0);
        Assert.True(routingSnapshot.Segments.Length > 0);
        Assert.True(routingSnapshot.SegmentNames.Length > 0);
        Assert.True(routingSnapshot.Downstreams.Length > 0);
        Assert.True(routingSnapshot.DownstreamRoutes.Length > 0);
        Assert.True(routingSnapshot.Hosts.Length > 0);
    }
}