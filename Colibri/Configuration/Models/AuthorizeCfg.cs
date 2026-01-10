namespace Colibri.Configuration.Models;

public sealed class AuthorizeCfg
{
    public required bool Enabled { get; set; }
    public required string PolicyId { get; set; }
    public required string[] Roles { get; set; }
}