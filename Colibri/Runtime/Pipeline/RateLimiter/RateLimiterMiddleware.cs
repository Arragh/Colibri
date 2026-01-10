namespace Colibri.Runtime.Pipeline.RateLimiter;

public sealed class RateLimiterMiddleware : IPipelineMiddleware
{
    private readonly RateLimiter _limiter = new();

    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("RATE LIMITER");
        
        if (!_limiter.Allow(ctx.ClusterId, ctx.EndpointId))
        {
            ctx.StatusCode = 429;
            ctx.IsCompleted = true;
            return ValueTask.CompletedTask;
        }

        return next(ctx);
    }
}