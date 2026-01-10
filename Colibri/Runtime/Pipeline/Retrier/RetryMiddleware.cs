namespace Colibri.Runtime.Pipeline.Retrier;

public sealed class RetryMiddleware : IPipelineMiddleware
{
    private readonly int _maxAttempts = 3;
    
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate next)
    {
        Console.WriteLine("RETRIER");
        
        for (int attempt = 1; attempt <= _maxAttempts; attempt++)
        {
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