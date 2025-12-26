using Colibri.Services.Middleware.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

namespace Colibri.Services.Middleware;

public sealed class TerminalMiddleware : IPipelineMiddleware
{
    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate _)
    {
        Console.WriteLine("Terminal Middleware Invoked");
        
        ctx.StatusCode = 200;
        ctx.IsCompleted = true;
        return ValueTask.CompletedTask;
    }
}