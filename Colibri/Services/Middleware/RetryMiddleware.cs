using Colibri.Services.Middleware.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware;

public sealed class RetryMiddleware : IPipelineMiddleware
{
    private readonly int _maxAttempts;

    public RetryMiddleware(int maxAttempts)
    {
        _maxAttempts = maxAttempts;
    }

    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        for (int attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            Console.WriteLine("Retry Middleware Invoked");
            
            ctx.Attempts = attempt;

            await next(ctx);

            if (ctx.StatusCode < 500)
            {
                return;
            }

            if (attempt == _maxAttempts)
            {
                return;
            }
        }
    }
}