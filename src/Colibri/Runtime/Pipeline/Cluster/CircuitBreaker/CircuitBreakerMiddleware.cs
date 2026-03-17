using System.Net;

namespace Colibri.Runtime.Pipeline.Cluster.CircuitBreaker;

public sealed class CircuitBreakerMiddleware(CircuitBreaker breaker) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        if (!breaker.CanExecute(ctx.HostIdx))
        {
            ctx.SetStatusCode(HttpStatusCode.ServiceUnavailable);
            return;
        }

        await next(ctx);

        breaker.ReportResult(
            ctx.HostIdx,
            ctx.StatusCode < 500);
    }
}