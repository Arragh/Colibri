using Colibri.Helpers;
using Colibri.Services.Pipeline.Interfaces;
using Colibri.Services.Pipeline.Models;

namespace Colibri.Services.Terminal;

public sealed class TerminalMiddleware : IPipelineMiddleware
{
    public async ValueTask InvokeAsync(
        PipelineContext ctx,
        PipelineDelegate _)
    {
        // Тут вообще весь код - одна большая заглушка
        
        var requestUri = new Uri(
            ctx.ClusterSnapshot.Clusters[ctx.ClusterId].Hosts[ctx.SelectedHost],
            ctx.HttpContext.Request.Path + ctx.HttpContext.Request.QueryString.Value);

        using var request = new HttpRequestMessage(HttpMethodCache.Get(ctx.HttpContext.Request.Method), requestUri);
        if (ctx.HttpContext.Request.ContentLength > 0 || ctx.HttpContext.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            request.Content = new StreamContent(ctx.HttpContext.Request.Body);
                            
            if (!string.IsNullOrEmpty(ctx.HttpContext.Request.ContentType))
            {
                request.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.HttpContext.Request.ContentType);
            }
        }
        request.Headers.ExpectContinue = false;
        
        using var response = await ctx.TransportSnapshot
            .Transports[ctx.ClusterId]
            .Invokers[ctx.SelectedHost]
            .SendAsync(request, ctx.HttpContext.RequestAborted);
        
        ctx.HttpContext.Response.StatusCode = (int)response.StatusCode;
        await response.Content.CopyToAsync(ctx.HttpContext.Response.Body, ctx.HttpContext.RequestAborted);
    }
}