using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public class RoutingSettings
{
    public ClusterDto[] Clusters { get; set; } = null!;
}