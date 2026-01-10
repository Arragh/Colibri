namespace Colibri.Configuration.Models;

public sealed class LoadBalancingCfg
{
    public required bool Enabled { get; set; }
    public required string Type { get; set; }
}