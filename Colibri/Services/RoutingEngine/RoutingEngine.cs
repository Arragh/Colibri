using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.RoutingState.Models;

namespace Colibri.Services.RoutingEngine;

public sealed class RoutingEngine : IRoutingEngine
{
    public bool TryMatchRoute(
        ReadOnlySpan<char> requestPath,
        ReadOnlySpan<char> method,
        RoutingSnapshot snapshot,
        out string downstreamPath)
    {
        for (int i = 0; i < snapshot.Clusters.Length; i++)
        {
            if (!requestPath.StartsWith(snapshot.Clusters[i].Prefix, StringComparison.Ordinal))
            {
                continue;
            }
            
            var slicedPath = requestPath.Slice(snapshot.Clusters[i].Prefix.Length);

            Console.WriteLine(slicedPath); // /users/5/info

            for (int j = 0; j < snapshot.Clusters[i].Endpoints.Length; j++)
            {
                if (!snapshot.Clusters[i].Endpoints[j].Method.AsSpan().Equals(method, StringComparison.Ordinal))
                {
                    continue;
                }
                
                // Тут что-то надо придумать
            }
        }
        
        downstreamPath = string.Empty;
        return false;
    }
}