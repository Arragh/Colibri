namespace Colibri.Configuration.Models;

public sealed class RouteCfg
{
    public string[] Methods
    {
        get;
        set => field = value
            .Select(v => v.ToUpperInvariant())
            .ToArray();
    } = null!;

    public string ClusterName
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;

    public string UpstreamPattern
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;

    public string DownstreamPattern
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = null!;
}