namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class TempCluster
{
    public string Protocol { get; set; }
    public string[] Hosts { get; set; }
    public TempRoute[] Routes { get; set; }
}