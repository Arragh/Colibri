using Colibri.Services.LoadBalancer.Interfaces;
using Colibri.Services.Middleware.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware;

public sealed class LoadBalancerMiddleware : IPipelineMiddleware
{
    private readonly ILoadBalancer _lb;

    public LoadBalancerMiddleware(ILoadBalancer lb)
    {
        _lb = lb;
    }

    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("Load Balancer Middleware Invoked");
        
        ctx.SelectedHost = _lb.SelectHost(ctx.ClusterId);
        return next(ctx);
    }
}