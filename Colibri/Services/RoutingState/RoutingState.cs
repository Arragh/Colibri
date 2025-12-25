using Colibri.Configuration;
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
            var oldSnapshot = _snapshot;
            var newSnapshot = Build(c);
            Volatile.Write(ref _snapshot, newSnapshot);
                
            // _disposer.Enqueue(oldSnapshot); // TODO: сделать диспозер!!!
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
                BaseUrls = c.BaseUrls,
                Endpoints = c.Endpoints.Select(e => new EndpointConfig
                {
                    Method = e.Method.ToUpperInvariant(),
                    Downstream = e.Downstream.ToLowerInvariant(),
                    Upstream = e.Upstream.ToLowerInvariant()
                }).ToArray()
            }).ToArray()
        };
    }
}