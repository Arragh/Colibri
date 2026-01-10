namespace Colibri.Configuration.Models;

public sealed class RetryCfg
{
    public bool Enabled { get; set; }
    public int Attempts { get; set; }
    public string Backoff { get; set; } = null!;
    public int[] RetryOn { get; set; } = null!;
}