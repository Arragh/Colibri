using Colibri.Interfaces.Services.Http;

namespace Colibri.Services.Http;

internal sealed class TheoryTransport(SocketsHttpHandler handler) : ITransport, IDisposable
{
    private readonly HttpMessageInvoker _invoker = new(handler);
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _invoker.SendAsync(request, cancellationToken);
    }

    public void Dispose()
    {
        _invoker.Dispose();
    }
}