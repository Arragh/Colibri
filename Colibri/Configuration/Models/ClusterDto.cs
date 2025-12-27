namespace Colibri.Configuration.Models;

public class ClusterDto
{
    public string Prefix { get; set; } = null!;
    public string Protocol { get; set; } = null!;
    public string[] Hosts { get; set; } = null!;
    public EndpointDto[] Endpoints { get; set; } = null!;
}