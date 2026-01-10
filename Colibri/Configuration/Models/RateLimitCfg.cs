namespace Colibri.Configuration.Models;

public sealed class RateLimitCfg
{
    public required bool Enabled { get; set; }
    public required int RequestsPerMinute { get; set; }
}