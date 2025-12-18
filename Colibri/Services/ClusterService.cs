using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class ClusterService(IOptions<ClusterSetting> cfg)
{
    private readonly string[] _prefixes = cfg.Value.GetPrefixes();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetClusterIndex(HttpContext ctx)
    {
        ReadOnlySpan<char> path = ctx.Request.Path.Value ?? ReadOnlySpan<char>.Empty;
        
        for (var i = 0; i < _prefixes.Length; i++)
        {
            if (path.StartsWith(_prefixes[i], StringComparison.Ordinal)
                && (path.Length == _prefixes[i].Length || path[_prefixes[i].Length] == '/'))
            {
                return i;
            }
        }
        
        return -1;
    }
}