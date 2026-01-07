namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class TempRoute
{
    public string Method { get; set; }
    public string[] Upstream { get; set; }
    public string Downstream { get; set; }
}