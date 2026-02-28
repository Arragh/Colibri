namespace Colibri.Configuration.Models;

public sealed class AuthCfg
{
    public bool Enabled { get; set; }
    public string PublicKey { get; set; } = string.Empty;
}