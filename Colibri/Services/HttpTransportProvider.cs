using System.Net;
using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

public sealed class HttpTransportProvider
{
    private readonly HttpMessageInvoker[] _transports;

    public HttpTransportProvider(IOptions<ClusterSetting> config)
    {
        var invokers = new List<HttpMessageInvoker>();
        
        foreach (var endpoint in config.Value.GetPrefixes())
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

            invokers.Add(new HttpMessageInvoker(handler, disposeHandler: false));
        }
        
        _transports = invokers.ToArray();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<HttpResponseMessage> SendAsync(
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await _transports[clusterIndex].SendAsync(request, cancellationToken);
    }
}