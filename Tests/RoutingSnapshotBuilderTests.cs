using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Tests;

public class RoutingSnapshotBuilderTests
{
    [Fact]
    public void Build_SingleCluster_SingleRoute_ResultNotNull()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
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
        
        // Assert
        Assert.NotNull(routingSnapshot);
    }
    
    [Fact]
    public void Build_MultipleClusters_MultipleRoutes_ResultNotNull()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = ["http://trololo2:5000"],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api2/test",
                            DownstreamPattern = "/internal2/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        // Assert
        Assert.NotNull(routingSnapshot);
    }

    [Fact]
    public void Build_SingleCluster_SingleHost_SetsHostsCorrectly()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
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
        var hosts = routingSnapshot.Hosts.ToArray();
        
        // Assert
        Assert.NotEmpty(hosts);
        Assert.Single(hosts);
    }
    
    [Fact]
    public void Build_SingleCluster_MultipleHosts_SetsHostsCorrectly()
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
                    Hosts = [
                        "http://trololo1:5000",
                        "http://trololo2:5000"
                    ],
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
        var hosts = routingSnapshot.Hosts.ToArray();
        
        // Assert
        Assert.NotEmpty(hosts);
        Assert.Equal(2, hosts.Length);
    }
    
    [Fact]
    public void Build_MultipleClusters_SingleHost_SetsHostsCorrectly()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo2:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api2/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var hosts = routingSnapshot.Hosts.ToArray();
        
        // Assert
        Assert.NotEmpty(hosts);
        Assert.Equal(2, hosts.Length);
    }
    
    [Fact]
    public void Build_MultipleClusters_MultipleHosts_SetsHostsCorrectly()
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
                    Hosts = [
                        "http://trololo:5000",
                        "http://trololo:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo2:5000",
                        "http://trololo2:5002"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api2/test",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var hosts = routingSnapshot.Hosts.ToArray();
        
        // Assert
        Assert.NotEmpty(hosts);
        Assert.Equal(4, hosts.Length);
    }

    [Fact]
    public void Build_SingleCluster_SingleRoute_SetsSegmentsCorrectly()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
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
        var segmentNames = routingSnapshot.SegmentNames;
        
        // Assert
        Assert.NotNull(segments.ToArray());
        Assert.NotNull(segmentNames.ToArray());
        
        Assert.Equal(
            2,
            segments.Length);
        
        Assert.Equal(
            9,
            segmentNames.Length);
        
        Assert.Equal(
            "/api",
            segmentNames.Slice(segments[0].PathStartIndex, segments[0].PathLength));
        
        Assert.Equal(
            "/test",
            segmentNames.Slice(segments[1].PathStartIndex, segments[1].PathLength));
    }
    
    [Fact]
    public void Build_SingleCluster_MultipleRoutes_SetsSegmentNamesCorrectly()
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
                    Hosts = [
                        "http://trololo:5000"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users",
                            DownstreamPattern = "/internal/test"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/account",
                            DownstreamPattern = "/internal/test"
                        },
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/new",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segments = routingSnapshot.Segments;
        var segmentNames = routingSnapshot.SegmentNames;
        
        // Assert
        Assert.NotNull(segments.ToArray());
        Assert.NotNull(segmentNames.ToArray());
        
        Assert.Equal(
            4,
            segments.Length);
        
        Assert.Equal(
            22,
            segmentNames.Length);
        
        Assert.Equal(
            "/api",
            segmentNames.Slice(segments[0].PathStartIndex, segments[0].PathLength));
        
        Assert.Equal(
            "/account",
            segmentNames.Slice(segments[1].PathStartIndex, segments[1].PathLength));
        
        Assert.Equal(
            "/users",
            segmentNames.Slice(segments[2].PathStartIndex, segments[2].PathLength));
        
        Assert.Equal(
            "/new",
            segmentNames.Slice(segments[3].PathStartIndex, segments[3].PathLength));
    }
    
    [Fact]
    public void Build_MultipleClusters_SingleRoute_SetsSegmentsCorrectly()
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
                    Hosts = [
                        "http://trololo:5001"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/find",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo:5002"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/account",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                },
                new ClusterDto
                {
                    Protocol = "Http",
                    Hosts = [
                        "http://trololo:5003"
                    ],
                    Routes =
                    [
                        new RouteDto
                        {
                            Method = "GET",
                            UpstreamPattern = "/api/users/create",
                            DownstreamPattern = "/internal/test"
                        }
                    ]
                }
            ]
        };
        
        // Act
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        var segments = routingSnapshot.Segments;
        var segmentNames = routingSnapshot.SegmentNames;
        
        // Assert
        Assert.NotNull(segments.ToArray());
        Assert.NotNull(segmentNames.ToArray());
        
        Assert.Equal(
            5,
            segments.Length);
        
        Assert.Equal(
            30,
            segmentNames.Length);
        
        Assert.Equal(
            "/api",
            segmentNames.Slice(segments[0].PathStartIndex, segments[0].PathLength));
        
        Assert.Equal(
            "/account",
            segmentNames.Slice(segments[1].PathStartIndex, segments[1].PathLength));
        
        Assert.Equal(
            "/users",
            segmentNames.Slice(segments[2].PathStartIndex, segments[2].PathLength));
        
        Assert.Equal(
            "/create",
            segmentNames.Slice(segments[3].PathStartIndex, segments[3].PathLength));
        
        Assert.Equal(
            "/find",
            segmentNames.Slice(segments[4].PathStartIndex, segments[4].PathLength));
    }
    
}