using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class RoutingSettings
{
    public ClusterCfg[] Clusters { get; set; } = null!;
    public RouteCfg[] Routes { get; set; } = null!;
}