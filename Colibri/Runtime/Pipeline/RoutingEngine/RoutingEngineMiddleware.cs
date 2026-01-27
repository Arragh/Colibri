using Colibri.Helpers;

namespace Colibri.Runtime.Pipeline.RoutingEngine;

public sealed class RoutingEngineMiddleware : IPipelineMiddleware
{
    private readonly UpstreamMatcher _upstreamMatcher = new();
    private readonly DownstreamPathBuilder _pathBuilder = new();
    
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        var routingSnapshot = ctx.GlobalSnapshot.RoutingSnapshot;
        var path = ctx.HttpContext.Request.Path.Value.AsSpan();
        Span<char> buffer = stackalloc char[path.Length];
        var normalizedPath = NormalizePath(path, buffer);
        
        if (!_upstreamMatcher.TryMatch(
                routingSnapshot,
                normalizedPath,
                HttpMethodMask.GetMask(ctx.HttpContext.Request.Method),
                routingSnapshot.RootSegmentsCount,
                out var clusterId,
                out var routeParams,
                out var downstreamFirstChildIndex,
                out var downstreamChildrenCount))
        {
            ctx.HttpContext.Response.StatusCode = 404;
            return;
        }
        
        var pathUrl = _pathBuilder.Build(
            ctx.GlobalSnapshot.RoutingSnapshot,
            normalizedPath,
            routeParams,
            downstreamFirstChildIndex,
            downstreamChildrenCount);
        
        ctx.DownstreamPath = pathUrl;
        
        var cluster = ctx.GlobalSnapshot.ClusterSnapshot.Clusters[clusterId];
        cluster.Take();
        try
        {
            await cluster.Pipeline.ExecuteAsync(ctx);
        }
        finally
        {
            cluster.Release();
        }
    }
    
    static ReadOnlySpan<char> NormalizePath(ReadOnlySpan<char> path, Span<char> buffer)
    {
        int x = 0;
    
        for (int i = 0; i < path.Length; i++)
        {
            char c = path[i];

            if (c == '/' && x > 0 && buffer[x - 1] == '/')
            {
                continue;
            }

            buffer[x++] = c;
        }

        if (x > 1 && buffer[x - 1] == '/')
        {
            x--;
        }

        return buffer[..x];
    }
}