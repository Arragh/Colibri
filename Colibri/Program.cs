using Colibri.Configuration;
using Colibri.Helpers;
using Colibri.Services.CircuitBreaker;
using Colibri.Services.CircuitBreaker.Interfaces;
using Colibri.Services.LoadBalancer;
using Colibri.Services.LoadBalancer.Interfaces;
using Colibri.Services.Terminal;
using Colibri.Services.RateLimiter;
using Colibri.Services.RateLimiter.Interfaces;
using Colibri.Services.Retrier;
using Colibri.Services.Pipeline;
using Colibri.Services.Pipeline.Models;
using Colibri.Services.SnapshotProvider;
using Colibri.Services.SnapshotProvider.Interfaces;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
builder.Services.AddSingleton<ILoadBalancer, LoadBalancer>();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();
builder.Services.AddSingleton<ISnapshotProvider, SnapshotProvider>();

builder.Services.AddSingleton<RateLimiterMiddleware>();
builder.Services.AddSingleton<RetryMiddleware>();
builder.Services.AddSingleton<CircuitBreakerMiddleware>();
builder.Services.AddSingleton<LoadBalancerMiddleware>();
builder.Services.AddSingleton<TerminalMiddleware>();

builder.Services.AddSingleton<UnstableTerminalMiddleware>();
// builder.Services.AddSingleton<RetryMiddleware>(_ => new RetryMiddleware(maxAttempts: 3));

builder.Services.AddSingleton<Pipeline>(sp => new Pipeline([
    sp.GetRequiredService<RateLimiterMiddleware>(),
    sp.GetRequiredService<RetryMiddleware>(),
    sp.GetRequiredService<CircuitBreakerMiddleware>(),
    sp.GetRequiredService<LoadBalancerMiddleware>(),
    sp.GetRequiredService<TerminalMiddleware>()
    // sp.GetRequiredService<UnstableTerminalMiddleware>()
]));

var app = builder.Build();

var pipeline = app.Services.GetRequiredService<Pipeline>();
var snapshotProvider = app.Services.GetRequiredService<ISnapshotProvider>();

app.Run(async ctx =>
{
    var globalSnapshot = snapshotProvider.GlobalSnapshot;
    var theorySnapshot = snapshotProvider.TheorySnapshot;

    var lol = new PipelineContext
    {
        HttpContext = ctx,
        ClusterSnapshot = globalSnapshot.ClusterSnapshot,
        TransportSnapshot = globalSnapshot.TransportSnapshot,
        CancellationToken = ctx.RequestAborted,
        ClusterId = HttpMethodCache.Get(ctx.Request.Method) == HttpMethod.Get ? 0 : 1, // Заглушка
        EndpointId = 0, // Заглушка
    };
    
    await pipeline.ExecuteAsync(lol);
});

app.Run();