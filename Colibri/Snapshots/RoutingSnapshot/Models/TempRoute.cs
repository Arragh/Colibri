namespace Colibri.Snapshots.RoutingSnapshot.Models;

public sealed class TempRoute
{
    public string Method { get; set; } = null!;
    public string[] UpstreamSegments { get; set; } = null!;
    public string DownstreamPattern { get; set; } = null!;
}