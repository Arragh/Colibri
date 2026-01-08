using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;
using Tests.Helpers;

namespace Tests.Tests.SnapshotBuilder.Segments;

public class SegmentsTests
{
    [Fact]
    public void Build_SegmentNames_NotEmpty()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segmentNames = routingSnapshot.SegmentNames;
        
        // Assert
        Assert.NotEmpty(segmentNames.ToArray());
    }

    [Fact]
    public void Build_AllSegments_NotEmpty()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segments = routingSnapshot.Segments;
        
        // Assert
        Assert.NotEmpty(segments.ToArray());
    }
    
    [Fact]
    public void Build_AllSegments_BasicFieldsAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segmentsAsArray = routingSnapshot.Segments.ToArray();
        
        // Assert
        for (int i = 0; i < segmentsAsArray.Length; i++)
        {
            Assert.True(segmentsAsArray[i].PathLength > 0);
            Assert.True(segmentsAsArray[i].PathStartIndex >= 0);
            Assert.True(segmentsAsArray[i].ChildrenCount >= 0);

            if (segmentsAsArray[i].DownstreamCount > 0)
            {
                Assert.NotEqual(0, segmentsAsArray[i].MethodMask);
            }
        }
    }
    
    [Fact]
    public void Build_AllSegments_ChildrenIndexesAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segmentsAsArray = routingSnapshot.Segments.ToArray();

        // Assert
        for (int i = 0; i < segmentsAsArray.Length; i++)
        {
            if (segmentsAsArray[i].ChildrenCount > 0)
            {
                Assert.InRange(
                    segmentsAsArray[i].FirstChildIndex,
                    0,
                    segmentsAsArray.Length - segmentsAsArray[i].ChildrenCount);
            }
        }
    }
    
    [Fact]
    public void Build_AllSegments_PathIndexesAreValid()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = RoutingSettingsHelper.MultipleClusters_MultipleRoutes();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segmentsAsArray = routingSnapshot.Segments.ToArray();
        var names = routingSnapshot.SegmentNames;

        // Assert
        for (int i = 0; i < segmentsAsArray.Length; i++)
        {
            var slice = names.Slice(
                segmentsAsArray[i].PathStartIndex,
                segmentsAsArray[i].PathLength);
            
            Assert.False(slice.IsEmpty);
            Assert.Equal('/', slice[0]);
            Assert.NotEqual('/', slice[^1]);
        }
    }
    
    [Fact]
    public void Build_SegmentDownstreams_AreWithinBounds()
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
                    downstreamsAsArray.Length - segmentsAsArray[i].DownstreamCount);
            }
        }
    }
    
}