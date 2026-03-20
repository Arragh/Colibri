namespace Colibri.Configuration.Models;

public sealed class AuthCfg
{
    public string JwtScheme { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public ClaimCfg[] Claims { get; set; } = [];
}