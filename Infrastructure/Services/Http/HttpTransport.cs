using Core.Interfaces.Services.Http;

namespace Infrastructure.Services.Http;

public class HttpTransport(HttpMessageInvoker invoker) : IHttpTransport
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await invoker.SendAsync(request, cancellationToken);
    }
}