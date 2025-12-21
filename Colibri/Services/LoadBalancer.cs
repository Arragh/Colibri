using System.Runtime.CompilerServices;
using Colibri.Models;

namespace Colibri.Services;

internal sealed class LoadBalancer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Uri GetClusterUrl(
        RoutingSnapshot snapshot,
        int clusterIndex)
    {
        return new Uri(snapshot.BaseUrls[clusterIndex][0]);
    }
}