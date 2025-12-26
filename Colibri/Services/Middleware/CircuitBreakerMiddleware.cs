using Colibri.Services.CircuitBreaker.Interfaces;
using Colibri.Services.Middleware.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware;

public sealed class CircuitBreakerMiddleware : IPipelineMiddleware
{
    private readonly ICircuitBreaker _breaker;

    public CircuitBreakerMiddleware(ICircuitBreaker breaker)
    {
        _breaker = breaker;
    }

    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("Circuit Breaker Middleware Invoked");
        
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