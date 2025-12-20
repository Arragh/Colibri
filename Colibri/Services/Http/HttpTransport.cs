using Colibri.Interfaces.Services.Http;

namespace Colibri.Services.Http;

internal sealed class HttpTransport(SocketsHttpHandler invoker) : ITransport, IDisposable
{
    private readonly HttpMessageInvoker _invoker = new(invoker, disposeHandler: true);
    private int _activities = 0;
    private bool _disposing = false;
    private readonly object _lock = new object();

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            if (_disposing)
            {
                throw new ObjectDisposedException("HttpTransport");
            }
            
            _activities++;
        }

        try
        {
            return await _invoker.SendAsync(request, cancellationToken);
        }
        finally
        {
            lock (_lock)
            {
                _activities--;
                Monitor.PulseAll(_lock);
            }
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _disposing = true;

            while (_activities > 0)
            {
                Monitor.Wait(_lock);
            }
        }
        
        _invoker.Dispose();
    }
}