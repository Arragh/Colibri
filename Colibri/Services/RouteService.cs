using Colibri.Configuration;
using Colibri.Models.Static;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class RouteService
{
    private string[] _prefixes;

    public RouteService(IOptionsMonitor<ClusterSetting> cfg)
    {
        _prefixes = cfg.CurrentValue.Prefixes();
        
        cfg.OnChange(m =>
        {
            Interlocked.Increment(ref HotReloadState.HotReloadCount);

            try
            {
                Interlocked.Exchange(ref _prefixes, m.Prefixes());
            }
            finally
            {
                Interlocked.Decrement(ref HotReloadState.HotReloadCount);
            }
        });
    }

    internal ReadOnlySpan<char> BuildRoute(int clusterIndex, HttpContext ctx)
    {
        return ctx.Request.Path.Value.AsSpan()[_prefixes[clusterIndex].Length..];
    }
}