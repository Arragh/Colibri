using Colibri.Runtime.Snapshots;

namespace Colibri.Runtime.Pipeline;

public sealed class PipelineContext
{
    public required GlobalSnapshot GlobalSnapshot { get; init; }
    public required HttpContext HttpContext { get; init; }
    public required CancellationToken CancellationToken { get; init; }
    public int ClusterId { get; set; }
    public ushort FirstClusterChildIndex { get; set; }
    public ushort ClusterChildrenCount { get; set; }
    
    public int EndpointId { get; set; }
    public int SelectedHost { get; set; }
    public int Attempts { get; set; }
    public bool IsCompleted { get; set; }
    public int StatusCode { get; set; }
}