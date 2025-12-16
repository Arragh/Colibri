using System.Collections.ObjectModel;
using System.Net;
using Infrastructure.Configuration;
using Interfaces.Services.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Http;

public class HttpTransportProvider : IHttpTransportProvider
{
    private readonly IReadOnlyDictionary<string, IHttpTransport> _transports;

    public HttpTransportProvider(IOptions<ClusterSetting> config)
    {
        var dict = new Dictionary<string, IHttpTransport>();
        
        foreach (var endpoint in config.Value.Clusters)
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
            var transport = new HttpTransport(invoker);
            dict.Add(endpoint.Key, transport); // TODO: возможно стоит использовать TryAdd
        }
        
        _transports = new ReadOnlyDictionary<string, IHttpTransport>(dict);
    }
    
    public IHttpTransport GetHttpTransport(string key)
    {
        return _transports[key];
    }
}