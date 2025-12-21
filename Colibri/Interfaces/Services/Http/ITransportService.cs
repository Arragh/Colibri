using Colibri.Models;

namespace Colibri.Interfaces.Services.Http;

internal interface ITransportService
{
    Task<HttpResponseMessage> SendAsync(
        RoutingSnapshot snapshot,
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken);
}