namespace Colibri.Runtime.Pipeline.Retrier;

public sealed class RetryMiddleware : IPipelineMiddleware
{
    private readonly int _maxAttempts = 3;
    
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        for (int i = 1; i <= _maxAttempts; i++)
        {
            ctx.Attempts = i;

            await next(ctx);

            if (ctx.StatusCode < 500)
            {
                return;
            }

            if (i == _maxAttempts)
            {
                return;
            }
        }
    }
}