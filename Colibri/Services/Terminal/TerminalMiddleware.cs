using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;
using Colibri.Services.RoutingEngine.Interfaces;

namespace Colibri.Services.Terminal;

public sealed class TerminalMiddleware(IRoutingEngine routingEngine) : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate _)
    {
        Console.WriteLine("Terminal Middleware Invoked");

        var lol = routingEngine.TryMatchRoute(
            ctx.HttpContext.Request.Path.Value.AsSpan(),
            ctx.HttpContext.Request.Method,
            ctx.Snapshot,
            out var downstreamPath);
        
        ctx.StatusCode = 200;
        ctx.IsCompleted = true;
        
        await ctx.HttpContext.Response.WriteAsync(
            $"Attempts={ctx.Attempts}, " +
            $"Status={ctx.StatusCode}, " +
            $"Host={ctx.SelectedHost}");
    }
}