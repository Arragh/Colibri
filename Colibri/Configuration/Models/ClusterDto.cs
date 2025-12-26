using Colibri.Services.UpstreamPipeline.Enums;

namespace Colibri.Configuration.Models;

public class ClusterDto
{
    public string Prefix { get; set; } = null!;
    public Protocol Protocol { get; set; }
    public string[] Hosts { get; set; } = null!;
    public EndpointDto[] Endpoints { get; set; } = null!;
}