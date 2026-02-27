namespace Colibri.Configuration.Models;

public sealed class CircuitBreakerCfg
{
    public bool Enabled { get; set; }
    public int Failures { get; set; } = 5;
    public int Timeout { get; set; } = 30;
}