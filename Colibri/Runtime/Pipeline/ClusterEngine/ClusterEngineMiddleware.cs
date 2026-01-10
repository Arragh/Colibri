namespace Colibri.Runtime.Pipeline.ClusterEngine;

public sealed class ClusterEngineMiddleware(ClusterEngine clusterEngine) : IPipelineMiddleware
{
    public ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        Console.WriteLine("CLUSTER ENGINE MIDDLEWARE");
        
        var cluster = clusterEngine.TryMatch("trololo");
        return cluster.Pipeline.ExecuteAsync(ctx);
    }
}