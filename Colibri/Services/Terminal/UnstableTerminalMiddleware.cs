using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;

namespace Colibri.Services.Terminal;

public sealed class UnstableTerminalMiddleware : IPipelineMiddleware
{
    public ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate _)
    {
        Console.WriteLine("Unstable Terminal Middleware Invoked");
        
        if (ctx.Attempts < 3)
        {
            Console.WriteLine($"Unstable Terminal Middleware returned ERR-500");
            
            ctx.StatusCode = 500;
        }
        else
        {
            Console.WriteLine($"Unstable Terminal Middleware returned OK-200");
            
            ctx.StatusCode = 200;
        }

        ctx.IsCompleted = true;
        return ValueTask.CompletedTask;
    }
}