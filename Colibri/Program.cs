using Colibri.BackgroundServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Colibri.Services;
using Colibri.Services.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ClusterSetting>().BindConfiguration("ClusterSetting");
builder.Services.PostConfigure<ClusterSetting>(c => c.BuildDictionaries());

builder.Services.AddSingleton<TransportDisposer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<TransportDisposer>());

builder.Services.AddSingleton<ITransportProvider, TheoryTransportProvider>();
builder.Services.AddSingleton<ClusterService>();
builder.Services.AddSingleton<LoadBalancer>();
builder.Services.AddSingleton<RouteService>();

var app = builder.Build();

app.Map("/{**catchAll}", static async (
    HttpContext ctx,
    ClusterService clusterService,
    LoadBalancer loadBalancer,
    RouteService routeService,
    ITransportProvider transportProvider) =>
{
    var clusterIndex = clusterService.GetClusterIndex(ctx);
    
    if (clusterIndex < 0)
    {
        ctx.Response.StatusCode = 404;
        return;
    }

    var clusterBaseUrl = loadBalancer.GetClusterUrl(clusterIndex);
    var pathUrl = routeService.BuildRoute(clusterIndex, ctx);

    var requestUri = new Uri(clusterBaseUrl, pathUrl.ToString() + ctx.Request.QueryString.Value);
    using var request = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), requestUri);

    if (ctx.Request.ContentLength > 0 || ctx.Request.Headers.ContainsKey("Transfer-Encoding"))
    {
        request.Content = new StreamContent(ctx.Request.Body);
                            
        if (!string.IsNullOrEmpty(ctx.Request.ContentType))
        {
            request.Content.Headers.TryAddWithoutValidation("Content-Type", ctx.Request.ContentType);
        }
    }
    request.Headers.ExpectContinue = false;
    using var response = await transportProvider.SendAsync(clusterIndex, request, ctx.RequestAborted);
    ctx.Response.StatusCode = (int)response.StatusCode;
    await response.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
});

app.Run();