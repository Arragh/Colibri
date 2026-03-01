namespace Colibri.Configuration.Models;

public sealed class ClusterCfg
{
    public bool Enabled { get; set; }
    public bool UsePrefix { get; set; }

    public string Name
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
    
    public AuthCfg? Authorization { get; set; }
    public RetryCfg? Retry { get; set; }
    public LoadBalancingCfg? LoadBalancer { get; set; }
    public CircuitBreakerCfg? CircuitBreaker { get; set; }
}