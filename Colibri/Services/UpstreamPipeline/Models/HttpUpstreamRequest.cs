using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Models;

public class HttpUpstreamRequest : UpstreamRequest
{
    public required string? ContentType { get; init; }
    public required long? ContentLength { get; init; }
    public required HttpMethod Method { get; init; }
    public required string PathAndQuery { get; init; }
}