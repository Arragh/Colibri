using System.Collections.ObjectModel;
using System.Net;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Http;

public sealed class HttpTransportProvider
{
    private readonly IReadOnlyDictionary<string, HttpTransport> _transports;

    public HttpTransportProvider(IOptions<ClusterSetting> config)
    {
        var dict = new Dictionary<string, HttpTransport>();
        
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
        
        _transports = new ReadOnlyDictionary<string, HttpTransport>(dict);
    }
    
    public HttpTransport GetTransport(string key)
    {
        return _transports[key];
    }
}