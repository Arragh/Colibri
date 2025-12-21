using Colibri.Interfaces.Services.Http;

namespace Colibri.Models;

internal sealed class RoutingSnapshot
{
    public RoutingSnapshot(ITransport[] transports, string[] prefixes, string[][] baseUrls)
    {
        Transports = transports;
        Prefixes = prefixes;
        BaseUrls = baseUrls;
    }
    
    public readonly ITransport[] Transports;
    public readonly string[] Prefixes;
    public readonly string[][] BaseUrls;
}