using System.Runtime.CompilerServices;
using Colibri.Models;

namespace Colibri.Services;

internal sealed class RoutingService
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetClusterIndex(
        RoutingSnapshot snapshot,
        HttpContext ctx)
    {
        ReadOnlySpan<char> path = ctx.Request.Path.Value ?? ReadOnlySpan<char>.Empty;
        
        for (var i = 0; i < snapshot.Prefixes.Length; i++)
        {
            if (path.StartsWith(snapshot.Prefixes[i], StringComparison.Ordinal)
                && (path.Length == snapshot.Prefixes[i].Length || path[snapshot.Prefixes[i].Length] == '/'))
            {
                return i;
            }
        }
        
        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<char> BuildRoute(
        RoutingSnapshot snapshot,
        HttpContext ctx,
        int clusterIndex)
    {
        return ctx.Request.Path.Value.AsSpan()[snapshot.Prefixes[clusterIndex].Length..];
    }
}