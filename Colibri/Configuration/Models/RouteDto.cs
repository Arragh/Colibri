namespace Colibri.Configuration.Models;

public class RouteDto
{
    public string Method { get; set; } = null!;
    public string UpstreamPattern { get; set; } = null!;
    public string DownstreamPattern { get; set; } = null!;
}