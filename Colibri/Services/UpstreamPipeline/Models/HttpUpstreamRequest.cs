using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Models;

public class HttpUpstreamRequest : UpstreamRequest
{
    public required HttpMethod Method { get; init; }
    public required string PathAndQuery { get; init; }
}