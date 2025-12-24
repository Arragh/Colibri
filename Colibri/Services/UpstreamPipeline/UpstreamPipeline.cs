using Colibri.Services.RoutingState.Models;
using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline;

public sealed class UpstreamPipeline : IUpstreamPipeline
{
    public Task<UpstreamResponse> ExecuteAsync(
        ClusterConfig clusterConfig,
        UpstreamRequest upstreamRequest,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}