namespace Colibri.Configuration.Models;

public sealed class ClusterCfg
{
    public required string ClusterId { get; set; }
    public required string Protocol { get; set; }
    public required string Prefix { get; set; }
    public required string[] Hosts { get; set; }
    public required bool Enabled { get; set; }
    public required AuthorizeCfg Authorize { get; set; }
    public required LoadBalancingCfg LoadBalancing { get; set; }
    public required CircuitBreakerCfg CircuitBreaker { get; set; }
    public required RetryCfg Retry { get; set; }
    public required RateLimitCfg RateLimit { get; set; }
    public required TimeoutCfg Timeouts { get; set; }
}