namespace Core.Interfaces.Services.Http;

public interface IHttpTransport
{
    Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken);
}