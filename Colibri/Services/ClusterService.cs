using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Colibri.Models.Static;
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
            Interlocked.Increment(ref HotReloadState.HotReloadCount);

            try
            {
                Interlocked.Exchange(ref _prefixes, s.Prefixes());
            }
            finally
            {
                Interlocked.Decrement(ref HotReloadState.HotReloadCount);
            }
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal int GetClusterIndex(HttpContext ctx)
    {
        ReadOnlySpan<char> path = ctx.Request.Path.Value ?? ReadOnlySpan<char>.Empty;
        
        var prefixes = Volatile.Read(ref _prefixes);
        
        for (var i = 0; i < prefixes.Length; i++)
        {
            if (path.StartsWith(prefixes[i], StringComparison.Ordinal)
                && (path.Length == prefixes[i].Length || path[prefixes[i].Length] == '/'))
            {
                return i;
            }
        }
        
        return -1;
    }
}