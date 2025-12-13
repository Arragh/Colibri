namespace Infrastructure.Configuration;

public sealed record EndpointsSettings
{
    public sealed record Endpoint
    {
        public string Key { get; init; }
        public string Host { get; init; }
        public int Port { get; init; }
    }
    
    public Endpoint[]  Endpoints { get; init; } = [];
}