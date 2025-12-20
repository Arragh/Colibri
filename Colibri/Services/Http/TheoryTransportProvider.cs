// using System.Net;
// using System.Runtime.CompilerServices;
// using Colibri.BackgroundServices;
// using Colibri.Configuration;
// using Colibri.Interfaces.Services.Http;
// using Microsoft.Extensions.Options;
//
// namespace Colibri.Services.Http;
//
// internal sealed class TheoryTransportProvider : ITransportProvider
// {
//     private ITransport[] _transports;
//     private readonly TransportDisposer _disposer;
//     
//     public TheoryTransportProvider(
//         IOptionsMonitor<ClusterSetting> cfg,
//         TransportDisposer disposer)
//     {
//         _transports = MakeArray(cfg.CurrentValue);
//         _disposer = disposer;
//         
//         cfg.OnChange(s =>
//         {
//             var oldTransports = Interlocked.Exchange(ref _transports, MakeArray(s));
//
//             foreach (var t in oldTransports)
//             {
//                 _disposer.Enqueue(t);
//             }
//         });
//     }
//     
//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public async Task<HttpResponseMessage> SendAsync(
//         int clusterIndex,
//         HttpRequestMessage request,
//         CancellationToken cancellationToken)
//     {
//         return await _transports[clusterIndex].SendAsync(request, cancellationToken);
//     }
//     
//     private ITransport[] MakeArray(ClusterSetting setting)
//     {
//         var invokers = new List<ITransport>();
//         
//         foreach (var _ in setting.Prefixes())
//         {
//             var handler = new SocketsHttpHandler
//             {
//                 MaxConnectionsPerServer = 2000,
//                 PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
//                 PooledConnectionLifetime = Timeout.InfiniteTimeSpan,
//                 ConnectTimeout = TimeSpan.FromSeconds(5),
//                 KeepAlivePingPolicy = HttpKeepAlivePingPolicy.Always,
//                 KeepAlivePingDelay = TimeSpan.FromSeconds(30),
//                 KeepAlivePingTimeout = TimeSpan.FromSeconds(5),
//                 AutomaticDecompression = DecompressionMethods.None,
//                 EnableMultipleHttp2Connections = true
//             };
//
//             invokers.Add(new HttpTransport(handler));
//         }
//         
//         return invokers.ToArray();
//     }
// }

using System.Net;
using System.Runtime.CompilerServices;
using Colibri.BackgroundServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Http;

internal sealed class TheoryTransportProvider : ITransportProvider
{
    private readonly Dictionary<string, ITransport> _transportsDict = new();
    private readonly TransportDisposer _disposer;
    private volatile ITransport[] _transports = null!;

    public TheoryTransportProvider(
        IOptionsMonitor<ClusterSetting> cfg,
        TransportDisposer disposer)
    {
        _disposer = disposer;

        Build(cfg.CurrentValue);
        cfg.OnChange(Build);
    }

    private void Build(ClusterSetting setting)
    {
        var newPrefixes = setting.Prefixes();
        var aliveClusters = new HashSet<string>(newPrefixes);
        var newTransports = new ITransport[newPrefixes.Length];

        for (var i = 0; i < newPrefixes.Length; i++)
        {
            var key = newPrefixes[i];

            if (!_transportsDict.TryGetValue(key, out var transport))
            {
                transport = CreateTransport();
                _transportsDict[key] = transport;
            }

            newTransports[i] = transport;
        }

        _transports = newTransports;

        foreach (var (idx, t) in _transportsDict.ToArray())
        {
            if (!aliveClusters.Contains(idx))
            {
                _transportsDict.Remove(idx);
                _disposer.Enqueue(t);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<HttpResponseMessage> SendAsync(
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => _transports[clusterIndex].SendAsync(request, cancellationToken);

    private static ITransport CreateTransport()
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

        return new TheoryTransport(handler);
    }
}