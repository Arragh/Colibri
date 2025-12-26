using Colibri.Services.Middleware.Interfaces;
using Colibri.Services.RateLimiter.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware;

public sealed class RateLimiterMiddleware : IPipelineMiddleware
{
    private readonly IRateLimiter _limiter;

    public RateLimiterMiddleware(IRateLimiter limiter)
    {
        _limiter = limiter;
    }

    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("Rate Limiter Middleware Invoked");
        
        if (!_limiter.Allow(ctx.ClusterId, ctx.EndpointId))
        {
            ctx.StatusCode = 429;
            ctx.IsCompleted = true;
            return ValueTask.CompletedTask;
        }

        return next(ctx);
    }
}