using Colibri.Runtime.Snapshots;
using Microsoft.AspNetCore.Http;

namespace Colibri.Runtime.Pipeline;

public sealed class PipelineContext
{
    public required GlobalSnapshot GlobalSnapshot { get; init; }
    public required HttpContext HttpContext { get; init; }
    public required CancellationToken CancellationToken { get; init; }
    public string DownstreamPath { get; set; }
    public bool IsHandled { get; set; }
    public int HostIdx { get; set; }
    public int StatusCode { get; set; }
}