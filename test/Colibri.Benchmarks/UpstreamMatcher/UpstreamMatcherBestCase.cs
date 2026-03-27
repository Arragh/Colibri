using BenchmarkDotNet.Attributes;
using Colibri.Configuration;
using Colibri.Configuration.Models;
using Colibri.Helpers;
using Colibri.Runtime.Pipeline.Main.RoutingEngine;
using Colibri.Runtime.Snapshots;
using Colibri.Runtime.Snapshots.Routing;
using Microsoft.Extensions.Caching.Memory;

namespace Colibri.Benchmarks.UpstreamMatcher;

[MemoryDiagnoser]
public class UpstreamMatcherBestCase
{
    private RoutingSnapshot _routingSnapshot = null!;
    private readonly Runtime.Pipeline.Main.RoutingEngine.UpstreamMatcher _matcher = new();
    
    [GlobalSetup]
    public void Setup()
    {
        _routingSnapshot = new GlobalSnapshotBuilder(new MemoryCache(new MemoryCacheOptions()))
            .Build(new ColibriSettings
            {
                Clusters =
                [
                    new ClusterCfg
                    {
                        Enabled = true,
                        Name = "cluster1",
                        Prefix = "/cluster1",
                        UsePrefix = true,
                        Protocol = "http",
                        Hosts = [ "127.0.0.1" ]
                    }
                ],
                Routes =
                [
                    new RouteCfg
                    {
                        ClusterName = "cluster1",
                        Methods = [ "GET" ],
                        UpstreamPattern = "/orders",
                        DownstreamPattern = "/api/v1/orders"
                    }
                ]
            }).RoutingSnapshot;

        ValidateTestUri();
    }

    private readonly string _requestUri = "/cluster1/orders";
    
    [Benchmark]
    public bool BestCaseTest()
    {
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
        
        var matchResult = _matcher.TryMatch(
            _routingSnapshot,
            _requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);
        
        return matchResult;
    }
    
    private void ValidateTestUri()
    {
        Span<ParamValue> routeParams = stackalloc ParamValue[16];
            
        var matchResult = _matcher.TryMatch(
            _routingSnapshot,
            _requestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            routeParams,
            out _,
            out _,
            out _);

        if (!matchResult)
        {
            throw new InvalidOperationException($"Could not find match for route {_requestUri}");
        }
    }
}

/*
   | Method       | Mean     | Error    | StdDev   | Allocated |
   |------------- |---------:|---------:|---------:|----------:|
   | BestCaseTest | 10.12 ns | 0.050 ns | 0.046 ns |         - | 
 */