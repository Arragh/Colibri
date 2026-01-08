namespace Colibri.Snapshots.RoutingSnapshot.Models;

public class TempCluster
{
    public string Protocol { get; set; } = null!;
    public string[] Hosts { get; set; } = null!;
    public TempRoute[] Routes { get; set; } = null!;
}