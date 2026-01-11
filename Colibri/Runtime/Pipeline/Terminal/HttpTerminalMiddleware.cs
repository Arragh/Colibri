using System.Net;

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
    
    public ValueTask InvokeAsync(PipelineContext ctx, PipelineDelegate next)
    {
        return next(ctx);
    }

    public void Dispose()
    {
        foreach (var invoker in _invokers)
        {
            invoker.Dispose();
        }
    }
}