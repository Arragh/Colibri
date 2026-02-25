namespace Colibri.Configuration.Models;

public sealed class CircuitBreakerCfg
{
    public bool Enabled { get; set; }
    public int Failures { get; set; }
    public int Timeout { get; set; }
}