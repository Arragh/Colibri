using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class ClusterService
{
    private string[] _prefixes;

    public ClusterService(IOptionsMonitor<ClusterSetting> cfg)
    {
        _prefixes = cfg.CurrentValue.Prefixes();

        cfg.OnChange(s =>
        {
            Interlocked.Exchange(ref _prefixes, s.Prefixes());
        });
    }

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