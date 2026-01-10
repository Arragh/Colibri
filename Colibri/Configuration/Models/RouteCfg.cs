namespace Colibri.Configuration.Models;

public sealed class RouteCfg
{
    public required string ClusterId { get; set; }
    public required string Method { get; set; }
    public required string UpstreamPattern { get; set; }
    public required string DownstreamPattern { get; set; }
}