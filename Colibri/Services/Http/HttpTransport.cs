namespace Colibri.Services.Http;

public sealed class HttpTransport(HttpMessageInvoker invoker)
{
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await invoker.SendAsync(request, cancellationToken);
    }
}