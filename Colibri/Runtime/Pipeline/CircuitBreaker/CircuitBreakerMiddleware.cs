namespace Colibri.Runtime.Pipeline.CircuitBreaker;

public sealed class CircuitBreakerMiddleware : IPipelineMiddleware
{
    private readonly CircuitBreaker _breaker = new();

    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("CIRCUIT BREAKER");
        
        if (!_breaker.CanExecute(ctx.ClusterId, ctx.EndpointId))
        {
            ctx.StatusCode = 503;
            ctx.IsCompleted = true;
            return;
        }

        await next(ctx);

        _breaker.ReportResult(
            ctx.ClusterId,
            ctx.EndpointId,
            ctx.StatusCode < 500);
    }
}