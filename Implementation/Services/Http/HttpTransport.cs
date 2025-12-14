using Interfaces.Services.Http;

namespace Implementation.Services.Http;

public class HttpTransport(HttpMessageInvoker invoker) : IHttpTransport
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return invoker.SendAsync(request, cancellationToken);
    }
}