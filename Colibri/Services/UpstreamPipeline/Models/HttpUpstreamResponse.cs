using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Models;

internal sealed class HttpUpstreamResponse : UpstreamResponse
{
    public required int StatusCode { get; init; }
    public required HttpResponseMessage HttpResponseMessage { get; init; }

    public override async Task CopyToAsync(HttpContext ctx, CancellationToken ct = default)
    {
        ctx.Response.StatusCode = StatusCode;

        foreach (var header in Headers)
        {
            ctx.Response.Headers[header.Key] = header.Value;
        }

        await using var responseStream = await HttpResponseMessage.Content.ReadAsStreamAsync(ct);
        await responseStream.CopyToAsync(ctx.Response.Body, ct);
    }
}