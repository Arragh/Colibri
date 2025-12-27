namespace Colibri.Configuration.Models;

public class EndpointDto
{
    public string Method { get; set; } = null!;
    public string Downstream { get; set; } = null!;
    public string Upstream { get; set; } = null!;
}