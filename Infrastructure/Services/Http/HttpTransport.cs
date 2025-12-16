using Interfaces.Services.Http;

namespace Infrastructure.Services.Http;

public class HttpTransport(HttpMessageInvoker invoker) : IHttpTransport
{
    public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return invoker.SendAsync(request, cancellationToken);
    }
}