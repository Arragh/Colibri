using Implementation.Configuration;
using Interfaces.Services.Http;
using Microsoft.Extensions.Options;

namespace Api.Extensions;

public static class MapColibriEndpointsExtension
{
    public static void MapColibriEndpoints(this WebApplication app)
    {
        var clusterSetting = app.Services
            .GetRequiredService<IOptions<ClusterSetting>>()
            .Value;
        
        foreach (var cluster in clusterSetting.Clusters)
        {
            foreach (var endpoint in cluster.Endpoints)
            {
                app
                    .MapMethods(
                        endpoint.OuterPath,
                        [endpoint.Method],
                        static async (HttpContext ctx, IHttpTransportProvider transportProvider) =>
                        {
                            var endpoint = ctx.GetEndpoint()!;
                            var clusterMeta = endpoint.Metadata.GetMetadata<ClusterMetadata>()!;
                            var endpointMeta = endpoint.Metadata.GetMetadata<EndpointMetadata>()!;
        
                            var transport = transportProvider.GetHttpTransport(clusterMeta.Key);
                            var requestUri = new Uri(clusterMeta.BaseUri, endpointMeta.InnerPath + ctx.Request.QueryString);

                            using var request = new HttpRequestMessage(endpointMeta.Method, requestUri);
        
                            if (ctx.Request.ContentLength > 0 || ctx.Request.Headers.ContainsKey("Transfer-Encoding"))
                            {
                                request.Content = new StreamContent(ctx.Request.Body);
                            
                                if (!string.IsNullOrEmpty(ctx.Request.ContentType))
                                {
                                    request.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.Request.ContentType);
                                }
                            }
        
                            request.Headers.ExpectContinue = false;
                            using var response = await transport.SendAsync(request, ctx.RequestAborted);
                            ctx.Response.StatusCode = (int)response.StatusCode;
                            await response.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
                        })
                    .WithMetadata(new ClusterMetadata(cluster.Key, new Uri(cluster.Host)))
                    .WithMetadata(new EndpointMetadata(endpoint.InnerPath, HttpMethod.Parse(endpoint.Method)));
            }
        }
    }
    
    private sealed record ClusterMetadata(string Key, Uri BaseUri);
    private sealed record EndpointMetadata(string InnerPath, HttpMethod Method);
}