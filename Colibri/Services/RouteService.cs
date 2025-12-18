using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class RouteService(IOptions<ClusterSetting> cfg)
{
    private readonly string[] _prefixes = cfg.Value.GetPrefixes();

    internal ReadOnlySpan<char> BuildRoute(int clusterIndex, HttpContext ctx)
    {
        return ctx.Request.Path.Value.AsSpan()[_prefixes[clusterIndex].Length..];
    }
}