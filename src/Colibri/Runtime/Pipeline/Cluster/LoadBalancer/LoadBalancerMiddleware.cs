namespace Colibri.Runtime.Pipeline.Cluster.LoadBalancer;

public sealed class LoadBalancerMiddleware(ILoadBalancer lb) : IPipelineMiddleware
{
    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        var hostIdx = lb.SelectHost();
        ctx.SetHostIdx(hostIdx);
        return next(ctx);
    }
}