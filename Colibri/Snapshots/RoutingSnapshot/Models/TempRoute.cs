namespace Colibri.Snapshots.RoutingSnapshot.Models;

public class TempRoute
{
    public string Method { get; set; } = null!;
    public string[] Upstream { get; set; } = null!;
    public string Downstream { get; set; } = null!;
}