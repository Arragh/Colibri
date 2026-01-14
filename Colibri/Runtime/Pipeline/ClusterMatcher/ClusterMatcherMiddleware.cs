namespace Colibri.Runtime.Pipeline.ClusterMatcher;

public sealed class ClusterMatcherMiddleware : IPipelineMiddleware
{
    private readonly ClusterMatcher _clusterMatcher = new();
    
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        var path = ctx.HttpContext.Request.Path.Value.AsSpan();
        
        if (!_clusterMatcher.TryMatch(
                path,
                ctx.GlobalSnapshot.RoutingSnapshot,
                out var clusterId,
                out var firstChildIndex,
                out var childrenCount))
        {
            ctx.HttpContext.Response.StatusCode = 404;
            return;
        }
        
        ctx.FirstClusterChildIndex = firstChildIndex;
        ctx.ClusterChildrenCount = childrenCount;
        
        var cluster = ctx.GlobalSnapshot.ClusterSnapshot.Clusters[clusterId];
        await cluster.Pipeline.ExecuteAsync(ctx);
    }
}