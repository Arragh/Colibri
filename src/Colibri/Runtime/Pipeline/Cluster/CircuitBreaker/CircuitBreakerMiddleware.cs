namespace Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;

public sealed class CircuitBreakerMiddleware(CircuitBreaker breaker) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        if (!breaker.CanExecute(ctx.HostIdx))
        {
            ctx.StatusCode = 503;
            return;
        }

        await next(ctx);

        breaker.ReportResult(
            ctx.HostIdx,
            ctx.StatusCode < 500);
    }
}