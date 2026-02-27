namespace Colibri.Runtime.Pipeline.Main.ClusterExecutor;

public sealed class ClusterExecutorMiddleware : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        var cluster = ctx.GlobalSnapshot.ClusterSnapshot.Clusters[ctx.ClusterId];
        
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
}