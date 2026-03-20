using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class ColibriSettings
{
    public JwtSchemeCfg[] JwtSchemes { get; set; } = null!;
    public ClusterCfg[] Clusters { get; set; } = null!;
    public RouteCfg[] Routes { get; set; } = null!;
}