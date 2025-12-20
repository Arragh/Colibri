using System.Net;
using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Http;

internal sealed class HttpTransportProvider : ITransportProvider
{
    private ITransport[] _transports;
    
    public HttpTransportProvider(IOptionsMonitor<ClusterSetting> cfg)
    {
        _transports = MakeArray(cfg.CurrentValue);

        cfg.OnChange(s =>
        {
            var oldTransports = Interlocked.Exchange(ref _transports, MakeArray(s));

            foreach (var t in oldTransports)
            {
                if (t is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        });
    }
    
    private ITransport[] MakeArray(ClusterSetting setting)
    {
        var invokers = new List<ITransport>();
        
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

            invokers.Add(new HttpTransport(handler));
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