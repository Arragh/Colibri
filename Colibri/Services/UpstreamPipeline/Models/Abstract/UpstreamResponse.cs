namespace Colibri.Services.UpstreamPipeline.Models.Abstract;

public abstract class UpstreamResponse
{
    public required IHeaderDictionary Headers { get; init; }
    public Stream? Body { get; init; }
}