using Colibri.Services.SnapshotProvider.Models.ClusterSnapshot;
using Colibri.Services.SnapshotProvider.Models.TransportSnapshot;

namespace Colibri.Services.Pipeline.Models;

public sealed class PipelineContext
{
    public required HttpContext HttpContext { get; init; }
    // public required ClusterConfigSnapshot ClusterSnapshot { get; init; }
    public required TransportRuntimeSnapshot TransportSnapshot { get; init; }
    public required CancellationToken CancellationToken { get; set; }
    
    public int ClusterId { get; init; }
    public int EndpointId { get; init; }

    public int SelectedHost { get; set; }
    public int Attempts { get; set; }

    public bool IsCompleted { get; set; }
    public int StatusCode { get; set; }
    
    // public long DeadlineTimestamp;
    // public bool IsExpired => Stopwatch.GetTimestamp() > DeadlineTimestamp;
    
    public Uri[] Hosts { get; set; }
    public string DownstreamPattern { get; set; }
}