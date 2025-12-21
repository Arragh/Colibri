using System.Net;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Colibri.Models;
using Colibri.Services.Http;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class RoutingState
{
    private RoutingSnapshot _snapshot;
    
    public RoutingState(IOptionsMonitor<ClusterSetting> cfg)
    {
        _snapshot = Build(cfg.CurrentValue);
        
        cfg.OnChange(c =>
        {
            _snapshot = Build(c);
        });
    }
    
    public RoutingSnapshot Snapshot => Volatile.Read(ref _snapshot);
    
    private static RoutingSnapshot Build(ClusterSetting setting) => new(
        MakeTransports(setting),
        MakePrefixes(setting),
        MakeBaseUrls(setting));
    
    private static ITransport[] MakeTransports(ClusterSetting setting)
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

    private static string[] MakePrefixes(ClusterSetting setting)
    {
        return setting.Clusters
            .Select(c => c.Prefix)
            .ToArray();
    }

    private static string[][] MakeBaseUrls(ClusterSetting setting)
    {
        return setting.Clusters
            .Select(c => c.BaseUrl)
            .ToArray();
    }
}