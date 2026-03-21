using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class ColibriSettings
{
    public JwtSchemeCfg[] JwtSchemes { get; set; } = [];
    public ClusterCfg[] Clusters { get; set; } = [];
    public RouteCfg[] Routes { get; set; } = [];
}