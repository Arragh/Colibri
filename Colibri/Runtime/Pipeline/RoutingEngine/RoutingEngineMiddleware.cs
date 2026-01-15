using Colibri.Helpers;

namespace Colibri.Runtime.Pipeline.RoutingEngine;

public sealed class RoutingEngineMiddleware : IPipelineMiddleware
{
    private readonly ClusterMatcher _clusterMatcher = new();
    private readonly UpstreamMatcher _upstreamMatcher = new();
    private readonly DownstreamPathBuilder _pathBuilder = new();
    
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        var path = ctx.HttpContext.Request.Path.Value.AsSpan();

        if (path[^1] == '/')
        {
            path = path[..^1];
        }
        
        if (!_clusterMatcher.TryMatch(
                path,
                ctx.GlobalSnapshot.RoutingSnapshot,
                out var clusterId,
                out var clusterLength,
                out var firstUpstreamIndex,
                out var upstreamsCount))
        {
            ctx.HttpContext.Response.StatusCode = 404;
            return;
        }

        path = path[clusterLength..];

        if (!_upstreamMatcher.TryMatch(
                ctx.GlobalSnapshot.RoutingSnapshot,
                path,
                HttpMethodMask.GetMask(ctx.HttpContext.Request.Method),
                firstUpstreamIndex,
                upstreamsCount,
                out var routeParams,
                out var downstreamFirstChildIndex,
                out var downstreamChildrenCount))
        {
            ctx.HttpContext.Response.StatusCode = 404;
            return;
        }
        
        var pathUrl = _pathBuilder.Build(
            ctx.GlobalSnapshot.RoutingSnapshot,
            path,
            routeParams,
            downstreamFirstChildIndex,
            downstreamChildrenCount);
        
        ctx.PathUrl = pathUrl;
        
        var cluster = ctx.GlobalSnapshot.ClusterSnapshot.Clusters[clusterId];
        await cluster.Pipeline.ExecuteAsync(ctx);
    }
}