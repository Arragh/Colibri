namespace Colibri.Configuration;

public sealed class ColibriSettings
{
    public required AuthorizationSettings Authorization { get; set; }
    public required RoutingSettings Routing { get; set; }
}