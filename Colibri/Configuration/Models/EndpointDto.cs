namespace Colibri.Configuration.Models;

public class EndpointDto
{
    public string Method
    {
        get;
        set => field = value.ToUpper();
    } = null!;

    public string Downstream
    {
        get;
        set => field = value.ToLower();
    } = null!;

    public string Upstream
    {
        get;
        set => field = value.ToLower();
    } = null!;
}