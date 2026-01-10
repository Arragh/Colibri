namespace Colibri.Configuration.Models;

public sealed class CircuitBreakerCfg
{
    public required bool Enabled { get; set; }
    public required int Failures { get; set; }
    public required int Timeout { get; set; }
}