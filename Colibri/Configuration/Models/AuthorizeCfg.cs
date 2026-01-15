namespace Colibri.Configuration.Models;

public sealed class AuthorizeCfg
{
    public bool Enabled { get; set; }
    public string PolicyId { get; set; } = null!;
    public string[] Roles { get; set; } = null!;
}