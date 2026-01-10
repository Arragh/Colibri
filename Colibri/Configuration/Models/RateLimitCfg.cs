namespace Colibri.Configuration.Models;

public sealed class RateLimitCfg
{
    public bool Enabled { get; set; }
    public int RequestsPerMinute { get; set; }
}