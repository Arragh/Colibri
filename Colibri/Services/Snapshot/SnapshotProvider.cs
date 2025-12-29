using System.Collections.Immutable;
using System.Net;
using Colibri.Configuration;
using Colibri.Services.Snapshot.Enums;
using Colibri.Services.Snapshot.Interfaces;
using Colibri.Services.Snapshot.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Services.Snapshot;

public class SnapshotProvider : ISnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;

    public SnapshotProvider(IOptionsMonitor<RoutingSettings> monitor)
    {
        _globalSnapshot = Build(monitor.CurrentValue);
        
        monitor.OnChange(c =>
        {
            var newGlobalSnapshot = Build(c);
            Volatile.Write(ref _globalSnapshot, newGlobalSnapshot);
        });
    }

    public ClusterConfigSnapshot ClusterSnapshot => Volatile.Read(ref _globalSnapshot).ClusterConfigSnapshot;
    public TransportRuntimeSnapshot TransportSnapshot => Volatile.Read(ref _globalSnapshot).TransportRuntimeSnapshot;
    
    private static GlobalSnapshot Build(RoutingSettings settings)
    {
        Console.WriteLine("SNAPSHOT CHANGED");
        
        return new GlobalSnapshot
        {
            ClusterConfigSnapshot = new ClusterConfigSnapshot
            {
                Clusters = settings.Clusters.Select(c => new ClusterConfig
                {
                    Prefix =  c.Prefix,
                    Protocol =  Enum.Parse<Protocol>(c.Protocol),
                    Hosts = c.Hosts.Select(h => new Uri(h)).ToImmutableArray(),
                    Endpoints = c.Endpoints.Select(e => new EndpointConfig
                    {
                        Method = e.Method.ToUpperInvariant(),
                        DownstreamPattern = e.DownstreamPattern.ToLowerInvariant(),
                        UpstreamPattern = e.UpstreamPattern.ToLowerInvariant(),
                        CachedUpstream = e.DownstreamPattern
                            .ToLowerInvariant()
                            .Split('/', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s =>
                            {
                                /*
                                 * Возможно потом имеет смысл переработать этот код для работы без аллокаций,
                                 * чтобы при перезагрузке конфига меньше нагружать GC,
                                 * но это на очень далекое будущее.
                                 */
                                if (s.StartsWith('{') && s.EndsWith('}'))
                                {
                                    return new RouteSegment(
                                        isParameter: true,
                                        parameterName: s.Replace("{", "").Replace("}", ""),
                                        literal: null
                                    );
                                }

                                return new RouteSegment(
                                    isParameter: false,
                                    parameterName: null,
                                    literal: s
                                );
                            }).ToImmutableArray()
                    }).ToImmutableArray()
                }).ToImmutableArray()
            },
            TransportRuntimeSnapshot = new TransportRuntimeSnapshot
            {
                Transports = settings.Clusters.Select(c => new TransportConfig
                {
                    Invokers = c.Hosts.Select(h => new HttpMessageInvoker(new SocketsHttpHandler
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
                    })).ToImmutableArray()
                }).ToImmutableArray()
            }
        };
    }
}