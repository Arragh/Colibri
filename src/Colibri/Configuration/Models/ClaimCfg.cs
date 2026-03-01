namespace Colibri.Configuration.Models;

public sealed class ClaimCfg
{
    public string Type { get; set; } = string.Empty;
    public string[] Value { get; set; } = Array.Empty<string>();
}