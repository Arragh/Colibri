using Colibri.Interfaces.Services.Http;

namespace Colibri.Services.Http;

internal sealed class HttpTransport(SocketsHttpHandler handler) : ITransport, IDisposable
{
    private readonly HttpMessageInvoker _invoker = new(handler, disposeHandler: true);
    private int _activities = 0;
    
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _activities);

        try
        {
            return await _invoker.SendAsync(request, cancellationToken);
        }
        finally
        {
            Interlocked.Decrement(ref _activities);
        }
    }
    
    public bool ReadyToDispose => Volatile.Read(ref _activities) == 0;

    public void Dispose()
    {
        _invoker.Dispose();
    }
}