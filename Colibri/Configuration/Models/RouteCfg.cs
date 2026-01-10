namespace Colibri.Configuration.Models;

public sealed class RouteCfg
{
    public string ClusterId { get; set; } = null!;
    public string Method { get; set; } = null!;
    public string UpstreamPattern { get; set; } = null!;
    public string DownstreamPattern { get; set; } = null!;
}