using System.Runtime.CompilerServices;
using Colibri.Services.RoutingState.Models;
using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamRequestFactory.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline;

public sealed class HttpUpstreamPipeline : IUpstreamPipeline
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Task<UpstreamResponse> ExecuteAsync(
        ClusterConfig clusterConfig,
        UpstreamRequest upstreamRequest)
    {
        /*
         * Полный пайплайн прохождения запроса через LoadBalancer, CircuitBreaker, Retry и т.д.
         * В конце возвращает ответ HttpUpstreamResponse (наследник от UpstreamResponse).
         */

        throw new NotImplementedException();
    }
}