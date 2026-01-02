using Colibri.Services.RateLimiter.Interfaces;
using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;

namespace Colibri.Services.RateLimiter;

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
        if (!_limiter.Allow(ctx.ClusterId, ctx.EndpointId))
        {
            ctx.StatusCode = 429;
            ctx.IsCompleted = true;
            return ValueTask.CompletedTask;
        }

        return next(ctx);
    }
}