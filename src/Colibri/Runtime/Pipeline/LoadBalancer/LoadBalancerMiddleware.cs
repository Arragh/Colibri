namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class LoadBalancerMiddleware(ILoadBalancer lb) : IPipelineMiddleware
{
    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        ctx.HostIdx = lb.SelectHost();
        return next(ctx);
    }
}