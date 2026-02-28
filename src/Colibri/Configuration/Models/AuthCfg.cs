namespace Colibri.Configuration.Models;

public sealed class AuthCfg
{
    public bool Enabled { get; set; }

    public string Algorithm
    {
        get;
        set => field = value.ToLowerInvariant();
    } = string.Empty;
    
    public string Key { get; set; } = string.Empty;
}