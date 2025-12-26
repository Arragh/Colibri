using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.RoutingEngine.Models;
using Colibri.Services.RoutingState.Interfaces;

namespace Colibri.Services.RoutingEngine;

public class RoutingEngine(IRoutingState routingState) : IRoutingEngine
{
    public RoutingMatchResult? Match(ReadOnlySpan<char> path, HttpMethod method)
    {
        /*
         * В снапшоте RoutingState ищет по префиксу кластер и возвращает его, либо null
         */

        foreach (var cluster in routingState.Snapshot.Clusters)
        {
            if (!path.StartsWith(cluster.Prefix.AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            
            var remainingPath = path[cluster.Prefix.Length..];

            foreach (var endpoint in cluster.Endpoints)
            {
                if (endpoint.Method.Equals(method.Method, StringComparison.OrdinalIgnoreCase) &&
                    remainingPath.Equals(endpoint.Downstream.AsSpan(), StringComparison.OrdinalIgnoreCase))
                {
                    return new RoutingMatchResult
                    {
                        Cluster = cluster,
                        Endpoint = endpoint
                    };
                }
            }
        }
        
        return null;
    }
}