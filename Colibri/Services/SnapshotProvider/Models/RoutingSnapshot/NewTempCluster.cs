namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class NewTempCluster
{
    public string Protocol { get; set; }
    public string[] Hosts { get; set; }
    public NewTempRoute[] Routes { get; set; }
}