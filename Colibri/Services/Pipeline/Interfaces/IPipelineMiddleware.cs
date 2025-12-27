using Colibri.Services.Pipeline.Models;

namespace Colibri.Services.Pipeline.Interfaces;

public interface IPipelineMiddleware
{
    ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next);
}

public delegate ValueTask PipelineDelegate(PipelineContext ctx);