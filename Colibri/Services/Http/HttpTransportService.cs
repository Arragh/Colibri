using System.Runtime.CompilerServices;
using Colibri.Interfaces.Services.Http;
using Colibri.Models;

namespace Colibri.Services.Http;

internal sealed class HttpTransportService : ITransportService
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<HttpResponseMessage> SendAsync(
        RoutingSnapshot  snapshot,
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await snapshot.Transports[clusterIndex].SendAsync(request, cancellationToken);
    }
}