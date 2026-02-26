namespace Colibri.Runtime.Pipeline;

public sealed class PipelineSrv : IDisposable
{
    private readonly PipelineDelegate _entry;
    private readonly IPipelineMiddleware[] _middlewares;

    public PipelineSrv(IPipelineMiddleware[] middlewares)
    {
        _middlewares = middlewares;
        
        PipelineDelegate next = _ => ValueTask.CompletedTask;

        for (int i = _middlewares.Length - 1; i >= 0; i--)
        {
            var current = _middlewares[i];
            var prev = next;
            next = ctx => current.InvokeAsync(ctx, prev);
        }

        _entry = next;
    }

    public ValueTask ExecuteAsync(PipelineContext ctx) => _entry(ctx);
    
    public void Dispose()
    {
        foreach (var middleware in _middlewares)
        {
            if (middleware is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}