using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Models;

public sealed class HttpUpstreamResponse : UpstreamResponse
{
    public required int StatusCode { get; init; }
}