using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class RoutingSettings
{
    public required ClusterCfg[] Clusters { get; set; }
    public required RouteCfg[] Routes { get; set; }
}