using Colibri.Configuration;
using Colibri.Helpers;
using Colibri.Services.CircuitBreaker;
using Colibri.Services.CircuitBreaker.Interfaces;
using Colibri.Services.HttpTransportPool;
using Colibri.Services.HttpTransportPool.Interfaces;
using Colibri.Services.LoadBalancer;
using Colibri.Services.LoadBalancer.Interfaces;
using Colibri.Services.RoutingEngine;
using Colibri.Services.RoutingEngine.Interfaces;
using Colibri.Services.RoutingState;
using Colibri.Services.RoutingState.Interfaces;
using Colibri.Services.UpstreamPipeline;
using Colibri.Services.UpstreamPipeline.Interfaces;
using Colibri.Services.UpstreamPipeline.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
builder.Services.AddSingleton<IHttpTransportPool, HttpTransportPool>();
builder.Services.AddSingleton<ILoadBalancer, LoadBalancer>();
builder.Services.AddSingleton<IRoutingEngine, RoutingEngine>();
builder.Services.AddSingleton<IRoutingState, RoutingState>();
builder.Services.AddSingleton<IUpstreamPipeline, HttpUpstreamPipeline>();
builder.Services.AddSingleton<IUpstreamPipelineProvider, UpstreamPipelineProvider>();

var app = builder.Build();

app.Map("/{**catchAll}", static async (
    IRoutingEngine routingEngine,
    IUpstreamPipelineProvider upstreamPipelineProvider,
    HttpContext ctx) =>
{
    var matchResult = routingEngine.Match(
        ctx.Request.Path,
        HttpMethodCache.Get(ctx.Request.Method));
    
    if (matchResult == null)
    {
        ctx.Response.StatusCode = 404;
        await ctx.Response.WriteAsync("Not Found");
        return;
    }

    var request = new HttpUpstreamRequest
    {
        ContentType = ctx.Request.ContentType,
        ContentLength = ctx.Request.ContentLength,
        Headers = ctx.Request.Headers,
        Body = ctx.Request.Body,
        Method = HttpMethodCache.Get(ctx.Request.Method),
        PathAndQuery = matchResult.Endpoint.Upstream + ctx.Request.QueryString,
        CancellationToken = ctx.RequestAborted
    };

    var upstreamPipeline = upstreamPipelineProvider.GetPipeline(matchResult.Cluster.Protocol);
    
    var upstreamResponse = await upstreamPipeline.ExecuteAsync(matchResult.Cluster, request);
    await upstreamResponse.CopyToAsync(ctx, request.CancellationToken);
});

app.Run();