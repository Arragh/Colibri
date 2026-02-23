namespace Colibri.Configuration.Models;

public sealed class LoadBalancingCfg
{
    public bool Enabled { get; set; }

    public string Type
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;
}