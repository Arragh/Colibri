using Colibri.Configuration;
using Colibri.Services.RoutingState.Enums;
using Colibri.Services.RoutingState.Interfaces;
using Colibri.Services.RoutingState.Models;
using Microsoft.Extensions.Options;

namespace Colibri.Services.RoutingState;

public sealed class RoutingState : IRoutingState
{
    private RoutingSnapshot _snapshot;

    public RoutingState(IOptionsMonitor<RoutingSettings> monitor)
    {
        _snapshot = Build(monitor.CurrentValue);
        
        monitor.OnChange(c =>
        {
            var newSnapshot = Build(c);
            Volatile.Write(ref _snapshot, newSnapshot);
        });
    }
    
    public RoutingSnapshot Snapshot => Volatile.Read(ref _snapshot);
    
    private static RoutingSnapshot Build(RoutingSettings settings)
    {
        Console.WriteLine("SNAPSHOT CHANGED");
        
        return new RoutingSnapshot
        {
            Clusters = settings.Clusters.Select(c => new ClusterConfig
            {
                Prefix =  c.Prefix,
                Protocol =  Enum.Parse<Protocol>(c.Protocol),
                Hosts = c.Hosts.Select(h => new Uri(h)).ToArray(),
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
                        }).ToArray()
                }).ToArray()
            }).ToArray()
        };
    }
}