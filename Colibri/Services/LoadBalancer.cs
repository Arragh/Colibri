using System.Runtime.CompilerServices;
using Colibri.Configuration;
using Microsoft.Extensions.Options;

namespace Colibri.Services;

internal sealed class LoadBalancer
{
    private string[][] _baseUrls;

    public LoadBalancer(IOptionsMonitor<ClusterSetting> cfg)
    {
        _baseUrls = cfg.CurrentValue.BaseUrls();
        
        cfg.OnChange(m =>
        {
            Interlocked.Exchange(ref _baseUrls, m.BaseUrls());
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Uri GetClusterUrl(int clusterIndex)
    {
        return new Uri(_baseUrls[clusterIndex][0]);
    }
}