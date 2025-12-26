using Colibri.Services.RoutingState.Models;
using Colibri.Services.UpstreamRequestFactory.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline.Interfaces;

public interface IUpstreamPipeline
{
    Task<UpstreamResponse> ExecuteAsync(
        ClusterConfig clusterConfig,
        UpstreamRequest upstreamRequest);
}