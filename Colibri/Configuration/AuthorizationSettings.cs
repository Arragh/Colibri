using Colibri.Configuration.Models;

namespace Colibri.Configuration;

public class AuthorizationSettings
{
    public PolicyDto[] Policies { get; set; } = null!;
}