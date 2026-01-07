using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Tests;

public class RoutingSnapshotBuilderTests
{
    [Fact]
    public void Build_NoSettings_CollectionsIsEmpty_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings();
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotNull(routingSnapshot);
        Assert.Empty(routingSnapshot.Segments.ToArray());
        Assert.Empty(routingSnapshot.Downstreams.ToArray());
        Assert.Empty(routingSnapshot.DownstreamRoutes.ToArray());
        Assert.Empty(routingSnapshot.SegmentNames.ToArray());
        
        Assert.Equal(
            0,
            routingSnapshot.RootSegmentsCount);
    }

    [Fact]
    public void Build_SingleCluster_SingleHost_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Hosts = ["http://ololo:5000"]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotEmpty(routingSnapshot.Hosts.ToArray());
        Assert.Single(routingSnapshot.Hosts.ToArray());
        
        Assert.Equal(
            new Uri("http://ololo:5000"),
            routingSnapshot.Hosts[0]);
    }
    
    [Fact]
    public void Build_SingleCluster_MultipleHosts_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Hosts = [
                        "http://ololo:5000",
                        "http://ololo:5001"
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.Equal(
            settings.Clusters[0].Hosts.Length,
            routingSnapshot.Hosts.Length);

        Assert.Equal(
            new Uri("http://ololo:5000"),
            routingSnapshot.Hosts[0]);
        
        Assert.Equal(
            new Uri("http://ololo:5001"),
            routingSnapshot.Hosts[1]);
    }

    [Fact]
    public void Build_MultipleClusters_SingleHost_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Hosts = [
                        "http://ololo:5000"
                    ]
                },
                new ClusterDto
                {
                    Hosts = [
                        "http://trololo:5000"
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotEmpty(routingSnapshot.Hosts.ToArray());
        
        Assert.Equal(
            settings.Clusters.SelectMany(cluster => cluster.Hosts).Count(),
            routingSnapshot.Hosts.Length);

        Assert.Equal(
            new Uri("http://ololo:5000"),
            routingSnapshot.Hosts[0]);
        
        Assert.Equal(
            new Uri("http://trololo:5000"),
            routingSnapshot.Hosts[1]);
    }
    
    [Fact]
    public void Build_MultipleClusters_MultipleHosts_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Hosts = [
                        "http://ololo:5000",
                        "http://ololo:5001"
                    ]
                },
                new ClusterDto
                {
                    Hosts = [
                        "http://trololo:5000",
                        "http://trololo:5001"
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotEmpty(routingSnapshot.Hosts.ToArray());
        
        Assert.Equal(
            settings.Clusters.SelectMany(cluster => cluster.Hosts).Count(),
            routingSnapshot.Hosts.Length);

        Assert.Equal(
            new Uri("http://ololo:5000"),
            routingSnapshot.Hosts[0]);
        
        Assert.Equal(
            new Uri("http://ololo:5001"),
            routingSnapshot.Hosts[1]);
        
        Assert.Equal(
            new Uri("http://trololo:5000"),
            routingSnapshot.Hosts[2]);
        
        Assert.Equal(
            new Uri("http://trololo:5001"),
            routingSnapshot.Hosts[3]);
    }
    
    [Fact]
    public void Build_SingleCluster_SingleRoute_WithoutParams_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = ["http://ololo:5000"],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segments = routingSnapshot.Segments;
        var downstreams = routingSnapshot.Downstreams;
        
        // Assert
        Assert.NotNull(routingSnapshot);
        Assert.Single(routingSnapshot.Hosts.ToArray());
        Assert.Single(routingSnapshot.Downstreams.ToArray());
        Assert.True(downstreams[0].MethodMask == HttpMethodMask.GetMask("GET"));
        
        Assert.Equal(
            new Uri("http://ololo:5000"),
            routingSnapshot.Hosts[0]);
        
        Assert.Equal(
            settings.Clusters.Length,
            routingSnapshot.RootSegmentsCount);
        
        Assert.Equal(
            2,
            routingSnapshot.Segments.Length);
        
        Assert.Equal(
            settings.Clusters[0].Routes[0].UpstreamPattern.Length,
            routingSnapshot.SegmentNames.Length);
        
        Assert.Equal(
            "/api",
            routingSnapshot.SegmentNames.Slice(segments[0].PathStartIndex, segments[0].PathLength));
        
        Assert.Equal(
            "/test",
            routingSnapshot.SegmentNames.Slice(segments[1].PathStartIndex, segments[1].PathLength));
        
        Assert.Equal(
            "/internal/test",
            routingSnapshot.DownstreamRoutes.Slice(downstreams[0].PathStartIndex, downstreams[0].PathLength));
    }

    [Fact]
    public void Build_MultipleClusters_SingleRoute_WithoutParams_NoError()
    {
        // Arrange
        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/ololo/api/trololo",
                            DownstreamPattern = "/internal1/test1"
                        }
                    ]
                },
                new ClusterDto
                {
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/trololo/ololo/api",
                            DownstreamPattern = "/internal2/test2"
                        }
                    ]
                }
            ]
        };
    }
}