using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public sealed class AuthorizationSettings
{
    public required PolicyCfg[] Policies { get; set; }
}