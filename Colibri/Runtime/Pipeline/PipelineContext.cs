using Colibri.Runtime.Snapshots;

namespace Colibri.Runtime.Pipeline;

public sealed class PipelineContext
{
    public required GlobalSnapshot GlobalSnapshot { get; init; }
    public required HttpContext HttpContext { get; init; }
    public required CancellationToken CancellationToken { get; init; }
    public int ClusterId { get; set; }
    public string DownstreamPath { get; set; }
    
    public int EndpointId { get; set; }
    public int HostIdx { get; set; }
    public int Attempts { get; set; }
    public bool IsCompleted { get; set; }
    public int StatusCode { get; set; }
}