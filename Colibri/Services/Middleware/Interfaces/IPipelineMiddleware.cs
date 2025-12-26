using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware.Interfaces;

public interface IPipelineMiddleware
{
    ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next);
}

public delegate ValueTask PipelineDelegate(PipelineContext ctx);