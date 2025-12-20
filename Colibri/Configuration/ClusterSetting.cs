namespace Colibri.Configuration;

internal sealed class ClusterSetting
{
    private string[] _prefixes = null!;
    private string[][] _baseUrls = null!;
    
    public Cluster[] Clusters { get; set; } = [];

    internal string[] Prefixes() => _prefixes;
    internal string[][]  BaseUrls() => _baseUrls;
    
    internal void BuildDictionaries()
    {
        _prefixes = Clusters
            .Select(c => c.Prefix)
            .ToArray();
        
        _baseUrls = Clusters
            .Select(c => c.BaseUrl)
            .ToArray();
    }
}

public sealed record Cluster
{
    public string Prefix { get; set; } = null!;

    public string[] BaseUrl { get; set; } = null!;
    
    public Endpoint[] Endpoints { get; set; } = [];
}

public sealed record Endpoint
{
    public string Method
    {
        get;
        set => field = value.ToUpper();
    } = null!;

    public string OuterPath
    {
        get;
        set => field = value.ToLower();
    } = null!;

    public string InnerPath
    {
        get;
        set => field = value.ToLower();
    } = null!;
}