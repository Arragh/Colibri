using Colibri.Helpers;
using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;
using Colibri.Services.RoutingEngine.Interfaces;

namespace Colibri.Services.RoutingEngine;

public sealed class RoutingEngineMiddleware(IRoutingEngine routingEngine) : IPipelineMiddleware
{
    public ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        if (!routingEngine.TryMatch(
                ctx.RoutingSnapshot,
                ctx.HttpContext.Request.Path.Value.AsSpan(),
                HttpMethodMask.GetMask(ctx.HttpContext.Request.Method),
                out var result))
        {
            ctx.HttpContext.Response.StatusCode = 404;
            return ValueTask.CompletedTask;
        }
        
        var hosts = ctx.RoutingSnapshot.Hosts
            .Slice(result!.Value.HostStartIndex, result.Value.HostsCount);

        ctx.DownstreamPattern = ctx.RoutingSnapshot.DownstreamRoutes
            .Slice(result.Value.PathStartIndex, result.Value.PathLength).ToString();

        ctx.Hosts = hosts.ToArray();
        
        return next(ctx);
    }
}