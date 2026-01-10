namespace Colibri.Configuration.Models;

public sealed class RetryCfg
{
    public required bool Enabled { get; set; }
    public required int Attempts { get; set; }
    public required string Backoff { get; set; }
    public required int[] RetryOn { get; set; }
}