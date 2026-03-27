namespace Colibri.Configuration.Models;

public sealed class RouteCfg
{
    public string[] Methods
    {
        get;
        set => field = value
            .Select(v => v.ToUpperInvariant())
            .ToArray();
    } = [];

    public string ClusterName
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = string.Empty;

    public string UpstreamPattern
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = string.Empty;

    public string DownstreamPattern
    {
        get;
        set => field = value
            .ToLowerInvariant();
    } = string.Empty;
}