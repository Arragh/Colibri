namespace Colibri.Services.UpstreamRequestFactory.Models.Abstract;

public abstract class UpstreamResponse
{
    public required IHeaderDictionary Headers { get; init; }
    public abstract Task CopyToAsync(HttpContext ctx, CancellationToken ct = default);
}