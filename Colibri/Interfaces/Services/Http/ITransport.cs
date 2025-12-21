namespace Colibri.Interfaces.Services.Http;

public interface ITransport : IDisposable
{
    bool ReadyToDispose { get; }
    
    Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken);
}