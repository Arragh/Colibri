namespace Colibri.Configuration.Models;

public sealed class RetryCfg
{
    public bool Enabled { get; set; }
    public int Attempts { get; set; } = 3;
}