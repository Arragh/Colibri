using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

namespace Tests;

public class RoutingSnapshotTests
{
    [Fact]
    public void Build_RoutingSnapshot()
    {
        var settings = new RoutingSettings
        {
            Clusters =
            [
                new ClusterDto
                {
                    Protocol = "http",
                    Hosts = ["http://localhost:5000"],
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

        var routingSnapshotBuilder = new RoutingSnapshotBuilder();
        var routingSnapshot = routingSnapshotBuilder.Build(settings);
        
        Assert.NotNull(routingSnapshot);

        var lol = settings.Clusters[0].Hosts.Select(h => new Uri(h)).ToArray();
        Assert.Equal(lol, routingSnapshot.Hosts);
    }
}