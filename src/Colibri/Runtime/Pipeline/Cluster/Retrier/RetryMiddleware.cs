namespace Colibri.Runtime.Pipeline.Cluster.Retrier;

public sealed class RetryMiddleware(int maxAttempts) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        for (int i = 1; i <= maxAttempts; i++)
        {
            await next(ctx);

            if (ctx.StatusCode < 500)
            {
                return;
            }

            if (i == maxAttempts)
            {
                return;
            }
        }
    }
}