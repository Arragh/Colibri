namespace Colibri.Runtime.Pipeline;

public interface IPipelineMiddleware
{
    ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next);
}