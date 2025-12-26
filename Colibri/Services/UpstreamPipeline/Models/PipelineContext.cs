namespace Colibri.Services.UpstreamPipeline.Models;

public sealed class PipelineContext
{
    public int ClusterId { get; init; }
    public int EndpointId { get; init; }

    public int SelectedHost { get; set; }
    public int Attempts { get; set; }

    public bool IsCompleted { get; set; }
    public int StatusCode { get; set; }
    
    public CancellationToken RequestAborted { get; set; }
    
    // public long DeadlineTimestamp;
    // public bool IsExpired => Stopwatch.GetTimestamp() > DeadlineTimestamp;
}