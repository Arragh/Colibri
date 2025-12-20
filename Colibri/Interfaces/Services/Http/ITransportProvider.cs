namespace Colibri.Interfaces.Services.Http;

public interface ITransportProvider
{
    Task<HttpResponseMessage> SendAsync(
        int clusterIndex,
        HttpRequestMessage request,
        CancellationToken cancellationToken);
}