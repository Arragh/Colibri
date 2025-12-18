using Colibri.Configuration;
using Colibri.Services.Http;
using Microsoft.Extensions.Options;

namespace Colibri.Extensions;

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
                        static async (
                            HttpContext ctx,
                            HttpTransportProvider transportProvider) =>
                        {
                            await ForwardAsync(ctx, transportProvider);
                        })
                    .WithMetadata(new EndpointMetadata(
                        cluster.Key,
                        new Uri(cluster.BaseUrl),
                        endpoint.InnerPath,
                        HttpMethod.Parse(endpoint.Method)));
            }
        }
    }

    private static async Task ForwardAsync(
        HttpContext ctx,
        HttpTransportProvider transportProvider)
    {
        var endpointMeta = ctx.GetEndpoint()!.Metadata.GetMetadata<EndpointMetadata>()!;
        
        var transport = transportProvider.GetTransport(endpointMeta.ClusterKey);
        var requestUri = new Uri(endpointMeta.BaseUri, endpointMeta.InnerPath + ctx.Request.QueryString);
        
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
    }
    
    private sealed record EndpointMetadata(string ClusterKey, Uri BaseUri, string InnerPath, HttpMethod Method);
}