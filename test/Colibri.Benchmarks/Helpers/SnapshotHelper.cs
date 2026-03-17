using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Runtime.Snapshots;

namespace Colibri.Benchmarks.Helpers;

public static class SnapshotHelper
{
    public static GlobalSnapshot CreateGlobalSnapshot()
    {
        var clusters = new ClusterCfg[]
        {
            new()
            {
                Enabled = true,
                Name = "cluster1",
                Prefix = "/cluster1",
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            }
        };

        var routes = new RouteCfg[]
        {
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/{id}/action1",
                DownstreamPattern = "/service1/{id}/action1",
            },
            new()
            {
                ClusterName = "cluster1",
                Methods = [ "GET" ],
                UpstreamPattern = "/route1/id/action1",
                DownstreamPattern = "/service1/id/action1",
            }
        };
        
        var settings = new ColibriSettings
        {
            Clusters = clusters,
            Routes = routes
        };
        
        return new GlobalSnapshotBuilder()
            .Build(settings);
    }
}