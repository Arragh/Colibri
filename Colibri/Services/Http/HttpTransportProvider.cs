using System.Net;
using System.Runtime.CompilerServices;
using Colibri.BackgroundServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Colibri.Models.Static;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Http;

internal sealed class HttpTransportProvider : ITransportProvider
{
    private ITransport[] _transports;
    private readonly TransportDisposer _disposer;
    
    public HttpTransportProvider(
        IOptionsMonitor<ClusterSetting> cfg,
        TransportDisposer disposer)
    {
        _transports = MakeArray(cfg.CurrentValue);
        _disposer = disposer;
        
        cfg.OnChange(s =>
        {
            Interlocked.Increment(ref HotReloadState.HotReloadCount);

            try
            {
                var oldTransports = Interlocked.Exchange(ref _transports, MakeArray(s));

                foreach (var t in oldTransports)
                {
                    _disposer.Enqueue(t);
                }
            }
            finally
            {
                Interlocked.Decrement(ref HotReloadState.HotReloadCount);
            }
        });
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<HttpResponseMessage> SendAsync(
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await _transports[clusterIndex].SendAsync(request, cancellationToken);
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
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(5),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
                KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                EnableMultipleHttp2Connections = true
            };

            invokers.Add(new HttpTransport(handler));
        }
        
        return invokers.ToArray();
    }
}