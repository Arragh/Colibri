using System.Collections.Immutable;
using System.Net;
using Colibri.Configuration;
using Colibri.Services.SnapshotProvider.Enums;
using Colibri.Services.SnapshotProvider.Interfaces;
using Colibri.Services.SnapshotProvider.Models;
using Colibri.Theory;
using Colibri.Theory.Structs;
using Microsoft.Extensions.Options;

namespace Colibri.Services.SnapshotProvider;

public class SnapshotProvider : ISnapshotProvider
{
    private GlobalSnapshot _globalSnapshot;
    private TheorySnapshotWrapper _theorySnapshotWrapper;
    
    private TheorySnapshotBuilder _snapshotBuilder;

    public SnapshotProvider(
        TheorySnapshotBuilder snapshotBuilder,
        IOptionsMonitor<RoutingSettings> monitor)
    {
        _snapshotBuilder = snapshotBuilder;
        _globalSnapshot = Build(monitor.CurrentValue);
        _theorySnapshotWrapper = _snapshotBuilder.BuildSnapshot(monitor.CurrentValue);
        
        monitor.OnChange(c =>
        {
            var newGlobalSnapshot = Build(c);
            Volatile.Write(ref _globalSnapshot, newGlobalSnapshot);
            
            var newTheorySnapshotWrapper = _snapshotBuilder.BuildSnapshot(c);
            Volatile.Write(ref _theorySnapshotWrapper, newTheorySnapshotWrapper);
        });
    }

    public ref readonly TheorySnapshot TheorySnapshot
    {
        get
        {
            var wrapper = Volatile.Read(ref _theorySnapshotWrapper);
            return ref wrapper.TheorySnapshot;
        }
    }

    public GlobalSnapshot GlobalSnapshot => Volatile.Read(ref _globalSnapshot);
    
    private static GlobalSnapshot Build(RoutingSettings settings)
    {
        Console.WriteLine("SNAPSHOT CHANGED");
        
        return new GlobalSnapshot
        {
            ClusterSnapshot = new ClusterConfigSnapshot
            {
                Clusters = settings.Clusters.Select(c => new ClusterConfig
                {
                    Protocol =  Enum.Parse<Protocol>(c.Protocol),
                    Hosts = c.Hosts.Select(h => new Uri(h)).ToImmutableArray(),
                    Routes = c.Routes.Select(e => new RouteConfig
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
            TransportSnapshot = new TransportRuntimeSnapshot
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