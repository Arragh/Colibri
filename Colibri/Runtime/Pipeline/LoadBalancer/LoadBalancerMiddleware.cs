namespace Colibri.Runtime.Pipeline.LoadBalancer;

public sealed class LoadBalancerMiddleware : IPipelineMiddleware
{
    private readonly LoadBalancer _lb = new();
    
    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        ctx.SelectedHost = _lb.SelectHost(ctx.ClusterId);
        return next(ctx);
    }
}