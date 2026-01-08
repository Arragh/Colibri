using Colibri.Snapshots.RoutingSnapshot;
using Tests.Helpers;

namespace Tests.Tests.SnapshotBuilder.Downstreams;

public class DownstreamsTests
{
    [Fact]
    public void Build_AllDownstreams_PathIndexesAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var downstreamsAsArray = routingSnapshot.Downstreams.ToArray();
        var routes = routingSnapshot.DownstreamRoutes;
        
        // Assert
        for (int i = 0; i < downstreamsAsArray.Length; i++)
        {
            Assert.True(downstreamsAsArray[i].PathLength > 0);
            Assert.InRange(
                downstreamsAsArray[i].PathStartIndex,
                0,
                routes.Length - downstreamsAsArray[i].PathLength);
        }
    }
    
    [Fact]
    public void Build_AllDownstreams_HostIndexesAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var downstreamsAsArray = routingSnapshot.Downstreams.ToArray();

        // Assert
        for (int i = 0; i < downstreamsAsArray.Length; i++)
        {
            Assert.InRange(
                downstreamsAsArray[i].HostStartIndex,
                0,
                routingSnapshot.Hosts.Length - downstreamsAsArray[i].HostsCount);
        }
    }
    
    [Fact]
    public void Build_AllDownstreams_MethodMaskIsValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var downstreamsAsArray = routingSnapshot.Downstreams.ToArray();

        for (int i = 0; i < downstreamsAsArray.Length; i++)
        {
            Assert.NotEqual(0, downstreamsAsArray[i].MethodMask);
        }
    }
    
    [Fact]
    public void Build_SegmentDownstream_IndexesAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segmentsAsArray = routingSnapshot.Segments.ToArray();
        var downstreamsAsArray = routingSnapshot.Downstreams.ToArray();
        
        // Assert
        for (int i = 0; i < segmentsAsArray.Length; i++)
        {
            if (segmentsAsArray[i].DownstreamCount > 0)
            {
                Assert.InRange(
                    segmentsAsArray[i].DownstreamStartIndex,
                    0,
                    routingSnapshot.Downstreams.Length - segmentsAsArray[i].DownstreamCount);
            }
        }
    }
}