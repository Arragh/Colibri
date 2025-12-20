namespace Colibri.Interfaces.Services.Http;

public interface ITransport
{
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
}