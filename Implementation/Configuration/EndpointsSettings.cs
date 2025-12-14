namespace Implementation.Configuration;

public sealed record EndpointsSettings
{
    public sealed record Endpoint
    {
        public string Key { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public Endpoint[] Endpoints { get; set; } = [];
}