using System.Net;
using Colibri.Helpers;

namespace Colibri.Runtime.Pipeline.Terminal;

public class HttpTerminalMiddleware : IPipelineMiddleware, IDisposable
{
    private readonly HttpMessageInvoker[] _invokers;
    private readonly Uri[] _uris;
    
    public HttpTerminalMiddleware(string[] hosts)
    {
        _invokers = new HttpMessageInvoker[hosts.Length];
        for (int i = 0; i < hosts.Length; i++)
        {
            var handler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 2000,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                EnableMultipleHttp2Connections = true
            };
            
            _invokers[i] = new HttpMessageInvoker(handler);
        }
        
        _uris = new Uri[hosts.Length];
        for (int i = 0; i < hosts.Length; i++)
        {
            _uris[i] = new Uri(hosts[i]);
        }
    }
    
    public async ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        var requestUri = new Uri(
            _uris[ctx.HostIdx],
            ctx.DownstreamPath + ctx.HttpContext.Request.QueryString);
        
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
        
        using var response = await _invokers[0]
            .SendAsync(request, ctx.HttpContext.RequestAborted);

        var responseStatusCode = (int)response.StatusCode;
        
        if (responseStatusCode < 500)
        {
            ctx.HttpContext.Response.StatusCode = responseStatusCode;
            ctx.StatusCode = responseStatusCode;
            await response.Content.CopyToAsync(ctx.HttpContext.Response.Body, ctx.HttpContext.RequestAborted);
        }
        else
        {
            ctx.StatusCode = (int)response.StatusCode;
        }
    }

    public void Dispose()
    {
        foreach (var invoker in _invokers)
        {
            invoker.Dispose();
        }
    }
}