using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public class ColibriSettings
{
    public ClusterDto[] Clusters { get; set; } = null!;
}