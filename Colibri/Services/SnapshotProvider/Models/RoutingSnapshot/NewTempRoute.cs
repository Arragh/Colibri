namespace Colibri.Services.SnapshotProvider.Models.RoutingSnapshot;

public class NewTempRoute
{
    public string Method { get; set; }
    public string[] Upstream { get; set; }
    public string Downstream { get; set; }
}