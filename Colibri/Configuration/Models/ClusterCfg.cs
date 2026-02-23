namespace Colibri.Configuration.Models;

public sealed class ClusterCfg
{
    public bool Enabled { get; set; }
    public string ClusterId { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public bool UsePrefix { get; set; }
    public string[] Hosts { get; set; } = null!;
    public LoadBalancingCfg LoadBalancing { get; set; } = null!;
    public CircuitBreakerCfg CircuitBreaker { get; set; } = null!;
    public RetryCfg Retry { get; set; } = null!;
}