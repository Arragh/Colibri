using BenchmarkDotNet.Attributes;
using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Runtime.Pipeline.Main.RoutingEngine;
using Colibri.Runtime.Snapshots;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Benchmarks.UpstreamMatcher;

[MemoryDiagnoser]
public class UpstreamMatcherRealCase
{
    private RoutingSnapshot _routingSnapshot = null!;
    private readonly Runtime.Pipeline.Main.RoutingEngine.UpstreamMatcher _matcher = new();

    public string[] RequestUris { get; } =
    [
        "/cluster1/users/123/profile",
        "/cluster5/orders",
        "/cluster7/orders/ORD-889900",
        "/cluster3/items/PRD-007",
        "/cluster10/analytics/reports/2025-04-05"
    ];

    /// <summary>
    /// Benchmarks UpstreamMatcher.TryMatch with realistic config: 10 clusters, 50 routes.
    /// </summary>
    [GlobalSetup]
    public void Setup()
    {
        var clusters = new List<ClusterCfg>();
        var routes = new List<RouteCfg>();

        for (int i = 1; i <= 10; i++)
        {
            var clusterName = $"cluster{i}";
            var clusterPrefix = $"/{clusterName}";
            
            clusters.Add(new ClusterCfg
            {
                Enabled = true,
                Name = clusterName,
                Prefix = clusterPrefix,
                UsePrefix = true,
                Protocol = "http",
                Hosts = [ "127.0.0.1" ]
            });
            
            routes.AddRange([
                new RouteCfg
                {
                    ClusterName = clusterName,
                    Methods = [ "GET", "POST" ],
                    UpstreamPattern = "/orders",
                    DownstreamPattern = "/api/v1/orders"
                },
                new RouteCfg
                {
                    ClusterName = clusterName,
                    Methods = [ "GET" ],
                    UpstreamPattern = "/users/{userId}/profile",
                    DownstreamPattern = "/api/v1/users/{userId}/profile"
                },
                new RouteCfg
                {
                    ClusterName = clusterName,
                    Methods = [ "GET" ],
                    UpstreamPattern = "/orders/{orderId}",
                    DownstreamPattern = "/api/v1/orders/{orderId}"
                },
                new RouteCfg
                {
                    ClusterName = clusterName,
                    Methods = [ "PUT", "DELETE", "GET" ],
                    UpstreamPattern = "/items/{itemId}",
                    DownstreamPattern = "/catalog/items/{itemId}"
                },
                new RouteCfg
                {
                    ClusterName = clusterName,
                    Methods = [ "GET" ],
                    UpstreamPattern = "/analytics/reports/{date}",
                    DownstreamPattern = "/reports/monthly/{date}"
                }
            ]);
        }

        var settings = new ColibriSettings
        {
            Clusters = clusters.ToArray(),
            Routes = routes.ToArray()
        };

        _routingSnapshot = new GlobalSnapshotBuilder()
            .Build(settings).RoutingSnapshot;

        ValidateTestUris();
    }
    
    [ParamsSource(nameof(RequestUris))]
    public string RequestUri { get; set; }
    
    [Benchmark]
    public bool RealCaseTests()
    {
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        var matchResult = _matcher.TryMatch(
            _routingSnapshot,
            RequestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        return matchResult;
    }

    private void ValidateTestUris()
    {
        foreach (var uri in RequestUris)
        {
            Span<ParamValue> routeParams = stackalloc ParamValue[16];
            
            var matchResult = _matcher.TryMatch(
                _routingSnapshot,
                uri.AsSpan(),
                HttpMethodMask.GetMask("GET"),
                routeParams,
                out _,
                out _,
                out _);

            if (!matchResult)
            {
                throw new InvalidOperationException($"Could not find match for route {uri}");
            }
        }
    }
}

/*
   | Method        | RequestUri           | Mean     | Error    | StdDev   | Allocated |
   |-------------- |--------------------- |---------:|---------:|---------:|----------:|
   | RealCaseTests | /clus(...)ofile [27] | 21.68 ns | 0.045 ns | 0.039 ns |         - |                                                                                                                            
   | RealCaseTests | /clus(...)04-05 [39] | 22.44 ns | 0.138 ns | 0.122 ns |         - |
   | RealCaseTests | /clus(...)D-007 [23] | 27.46 ns | 0.181 ns | 0.161 ns |         - |
   | RealCaseTests | /cluster5/orders     | 18.69 ns | 0.013 ns | 0.012 ns |         - |
   | RealCaseTests | /clus(...)89900 [27] | 32.89 ns | 0.009 ns | 0.008 ns |         - |
 */