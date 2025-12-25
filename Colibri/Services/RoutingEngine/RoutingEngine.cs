using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.RoutingState.Interfaces;
using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingEngine;

public class RoutingEngine(IRoutingState routingState) : IRoutingEngine
{
    public ClusterConfig? Match(ReadOnlySpan<char> path)
    {
        /*
         * В снапшоте в RoutingState ищет по префиксу кластер и возвращает его, либо null
         */

        if (path.Length == 9)
        {
            return routingState.Snapshot.Clusters[0];
        }
        else if (path.Length == 11)
        {
            return routingState.Snapshot.Clusters[1];
        }
        
        return null;
    }
}