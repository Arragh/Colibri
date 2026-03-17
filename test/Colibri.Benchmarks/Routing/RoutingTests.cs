using BenchmarkDotNet.Attributes;
using Colibri.Benchmarks.Helpers;
using Colibri.Helpers;
using Colibri.Runtime.Pipeline.Main.RoutingEngine;
using Colibri.Runtime.Snapshots.Routing;

namespace Colibri.Benchmarks.Routing;

[MemoryDiagnoser]
public class RoutingTests
{
    private readonly RoutingSnapshot _routingSnapshot = SnapshotHelper.CreateGlobalSnapshot().RoutingSnapshot;
    private readonly UpstreamMatcher _matcher = new();
    private const string RequestUri = "/cluster1/route1/id/action1";
    
    [Benchmark]
    public bool Trololo()
    {
        var matchResult = _matcher.TryMatch(
            _routingSnapshot,
            RequestUri.AsSpan(),
            HttpMethodMask.GetMask("GET"),
            out _,
            out _,
            out _,
            out _);
        
        return matchResult;
    }
}