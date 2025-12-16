namespace Infrastructure.Configuration;

public sealed record ClusterSetting
{
    public Cluster[] Clusters { get; set; } = [];
}

public sealed record Cluster
{
    public string Key { get; set; }
    public string Host { get; set; }
    
    public Endpoint[] Endpoints { get; set; } = [];
}

public sealed record Endpoint
{
    public string Method
    {
        get;
        set => field = value.ToUpper();
    }

    public string OuterPath
    {
        get;
        set => field = value.ToLower();
    }
    
    public string InnerPath
    {
        get;
        set => field = value.ToLower();
    }
}