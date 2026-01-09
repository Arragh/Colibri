namespace Colibri.Configuration;

public class ColibriSettings
{
    public AuthorizationSettings Authorization { get; set; } = null!;
    public RoutingSettings Routing { get; set; } = null!;
}