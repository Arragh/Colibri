using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class LoadBalancer(IOptions<ClusterSetting> cfg)
{
    private readonly string[][] _baseUrls = cfg.Value.GetBaseUrls();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Uri GetClusterUrl(int clusterIndex)
    {
        return new Uri(_baseUrls[clusterIndex][0]);
    }
}