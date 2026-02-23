namespace Colibri.Configuration.Models;

public sealed class ClusterCfg
{
    public bool Enabled { get; set; }
    public bool UsePrefix { get; set; }

    public string ClusterId
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;

    public string Protocol
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;

    public string Prefix
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;

    public string[] Hosts
    {
        get;
        set => field = value
            .Select(v => v.ToLowerInvariant())
            .ToArray();
    } = null!;
    
    public LoadBalancingCfg LoadBalancing { get; set; } = null!;
    public CircuitBreakerCfg CircuitBreaker { get; set; } = null!;
    public RetryCfg Retry { get; set; } = null!;
}