using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class AuthorizationSettings
{
    public PolicyCfg[] Policies { get; set; } = null!;
}