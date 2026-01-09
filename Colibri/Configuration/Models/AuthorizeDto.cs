namespace Colibri.Configuration.Models;

public class AuthorizeDto
{
    public bool Required { get; set; }
    public string PolicyId { get; set; } = null!;
    public string[] Roles { get; set; } = null!;
}