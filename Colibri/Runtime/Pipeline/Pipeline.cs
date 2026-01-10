namespace Colibri.Runtime.Pipeline;

public sealed class Pipeline
{
    private readonly PipelineDelegate _entry;

    public Pipeline(IPipelineMiddleware[] middlewares)
    {
        PipelineDelegate next = _ => ValueTask.CompletedTask;

        foreach (var m in middlewares.Reverse())
        {
            var current = m;
            var prev = next;
            next = ctx => current.InvokeAsync(ctx, prev);
        }

        _entry = next;
    }

    public ValueTask ExecuteAsync(PipelineContext ctx) => _entry(ctx);
}