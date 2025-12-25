using System.Runtime.CompilerServices;
using Colibri.Services.CircuitBreaker.Interfaces;
using Colibri.Services.HttpTransportPool.Interfaces;
using Colibri.Services.LoadBalancer.Interfaces;
using Colibri.Services.RoutingState.Models;
using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;
using Colibri.Services.UpstreamPipeline.Models.Abstract;

namespace Colibri.Services.UpstreamPipeline;

public sealed class HttpUpstreamPipeline : IUpstreamPipeline
{
    private readonly ILoadBalancer _loadBalancer;
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly IHttpTransportPool _transportPool;

    public HttpUpstreamPipeline(
        ILoadBalancer loadBalancer,
        ICircuitBreaker circuitBreaker,
        IHttpTransportPool httpTransportPool)
    {
        _loadBalancer = loadBalancer;
        _circuitBreaker = circuitBreaker;
        _transportPool = httpTransportPool;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public async Task<UpstreamResponse> ExecuteAsync(
        ClusterConfig clusterConfig,
        UpstreamRequest upstreamRequest)
    {
        /*
         * Полный пайплайн прохождения запроса через LoadBalancer, CircuitBreaker, Retry и т.д.
         * В конце возвращает ответ HttpUpstreamResponse (наследник от UpstreamResponse).
         */

        var invoker = _transportPool.GetHttpInvoker(clusterConfig.Prefix);

        if (upstreamRequest is HttpUpstreamRequest httpRequest)
        {
            // var uri = new Uri(clusterConfig.BaseUrls[0] + httpRequest.PathAndQuery);
            // var request = new HttpRequestMessage(httpRequest.Method, uri);
            
            var request =  CreateHttpRequest(httpRequest, clusterConfig);
            
            var response = await invoker.SendAsync(request, CancellationToken.None);

            return new HttpUpstreamResponse
            {
                Headers = new HeaderDictionary(),
                StatusCode = (int)response.StatusCode,
                HttpResponseMessage = response
            };
        }
        
        throw new NotImplementedException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static HttpRequestMessage CreateHttpRequest(HttpUpstreamRequest httpRequest, ClusterConfig clusterConfig)
    {
        var uri = new Uri(clusterConfig.BaseUrls[0] + httpRequest.PathAndQuery);
        return new HttpRequestMessage(httpRequest.Method, uri);
    }
}