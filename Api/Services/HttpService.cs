using System.Net;
using Api.Configuration;
using Api.Interfaces;
using Microsoft.Extensions.Options;

namespace Api.Services;

public class HttpService : IHttpService
{
    private readonly Dictionary<string, HttpMessageInvoker> _clients = new();

    public HttpService(IOptions<EndpointsSettings> config)
    {
        foreach (var endpoint in config.Value.Endpoints)
        {
            var handler = new SocketsHttpHandler
            {
                MaxConnectionsPerServer = 2000,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
                ConnectTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                AutomaticDecompression = DecompressionMethods.None,
                EnableMultipleHttp2Connections = true
            };

            var invoker = new HttpMessageInvoker(handler, disposeHandler: false);
            _clients[endpoint.Key] = invoker;
        }
    }

    public HttpMessageInvoker GetClient(string key)
    {
        return _clients[key];
    }
}