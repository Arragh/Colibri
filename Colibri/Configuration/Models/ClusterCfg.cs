namespace Colibri.Configuration.Models;

public sealed class ClusterCfg
{
    public string ClusterId { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public string[] Hosts { get; set; } = null!;
    public bool Enabled { get; set; }
    public AuthorizeCfg Authorize { get; set; } = null!;
    public LoadBalancingCfg LoadBalancing { get; set; } = null!;
    public CircuitBreakerCfg CircuitBreaker { get; set; } = null!;
    public RetryCfg Retry { get; set; } = null!;
    public RateLimitCfg RateLimit { get; set; } = null!;
    public TimeoutCfg Timeouts { get; set; } = null!;
}