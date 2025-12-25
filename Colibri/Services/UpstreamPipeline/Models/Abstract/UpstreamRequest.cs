namespace Colibri.Services.UpstreamPipeline.Models.Abstract;

public abstract class UpstreamRequest
{
    public required IHeaderDictionary Headers { get; init; }
    public Stream? Body { get; init; }
    public required CancellationToken CancellationToken { get; init; }
}