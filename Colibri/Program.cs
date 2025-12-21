using Colibri.BackgroundServices;
using Colibri.Configuration;
using Colibri.Interfaces.Services.Http;
using Colibri.Models.Static;
using Colibri.Services;
using Colibri.Services.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ClusterSetting>().BindConfiguration("ClusterSetting");
builder.Services.PostConfigure<ClusterSetting>(c => c.BuildDictionaries());

builder.Services.AddSingleton<SnapshotDisposer>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<SnapshotDisposer>());

builder.Services.AddSingleton<RoutingState>();
builder.Services.AddSingleton<ITransportService, HttpTransportService>();
builder.Services.AddSingleton<LoadBalancer>();
builder.Services.AddSingleton<RoutingService>();

var app = builder.Build();

app.Use((ctx, next) =>
{
    if (HotReloadState.HotReloadCount == 0)
    {
        return next(ctx);
    }
    
    ctx.Response.StatusCode = 503;
    
    return ctx.Response.Body.WriteAsync(
        HotReloadState.Server503Message,
        0,
        HotReloadState.Server503Message.Length,
        ctx.RequestAborted);
    
});

app.Map("/{**catchAll}", static async (
    HttpContext ctx,
    RoutingState routingState,
    LoadBalancer loadBalancer,
    RoutingService routingService,
    ITransportService transportProvider) =>
{
    var snapshot = routingState.Snapshot;
    
    var clusterIndex = routingService.GetClusterIndex(snapshot, ctx);
    
    if (clusterIndex < 0)
    {
        ctx.Response.StatusCode = 404;
        return;
    }

    var clusterBaseUrl = loadBalancer.GetClusterUrl(snapshot, clusterIndex);
    var pathUrl = routingService.BuildRoute(snapshot, ctx, clusterIndex);

    var requestUri = new Uri(
        clusterBaseUrl,
        pathUrl.ToString() + ctx.Request.QueryString.Value);
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
    using var response = await transportProvider.SendAsync(snapshot, clusterIndex, request, ctx.RequestAborted);
    ctx.Response.StatusCode = (int)response.StatusCode;
    await response.Content.CopyToAsync(ctx.Response.Body, ctx.RequestAborted);
});

app.Run();