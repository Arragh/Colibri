namespace Colibri.Configuration.Models;

public class ClusterDto
{
    public string Protocol { get; set; } = null!;
    public string[] Hosts { get; set; } = null!;
    public RouteDto[] Routes { get; set; } = null!;
}