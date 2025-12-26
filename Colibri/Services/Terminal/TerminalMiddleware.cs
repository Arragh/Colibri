using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;

namespace Colibri.Services.Terminal;

public sealed class TerminalMiddleware : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate _)
    {
        Console.WriteLine("Terminal Middleware Invoked");
        
        ctx.StatusCode = 200;
        ctx.IsCompleted = true;
        
        await ctx.HttpContext.Response.WriteAsync(
            $"Attempts={ctx.Attempts}, " +
            $"Status={ctx.StatusCode}, " +
            $"Host={ctx.SelectedHost}");
    }
}