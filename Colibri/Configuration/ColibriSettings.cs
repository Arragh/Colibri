namespace Colibri.Configuration;

public sealed class ColibriSettings
{
    public AuthorizationSettings Authorization { get; set; } = null!;
    public RoutingSettings Routing { get; set; } = null!;
}