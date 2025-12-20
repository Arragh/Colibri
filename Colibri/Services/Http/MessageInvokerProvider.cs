using System.Net;
using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Http;

internal sealed class MessageInvokerProvider : ITransportProvider
{
    private HttpMessageInvoker[] _transports;

    public MessageInvokerProvider(IOptionsMonitor<ClusterSetting> cfg)
    {
        _transports = MakeArray(cfg.CurrentValue);
    }

    private HttpMessageInvoker[] MakeArray(ClusterSetting setting)
    {
        var invokers = new List<HttpMessageInvoker>();
        
        foreach (var _ in setting.Prefixes())
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
        
        return invokers.ToArray();
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