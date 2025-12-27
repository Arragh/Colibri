using Colibri.Configuration;
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

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
builder.Services.AddSingleton<ILoadBalancer, LoadBalancer>();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();

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

app.Run(async ctx =>
{
    var lol = new PipelineContext
    {
        HttpContext = ctx,
        CancellationToken = ctx.RequestAborted,
        ClusterId = 1,
        EndpointId = 42
    };
    
    await pipeline.ExecuteAsync(lol);
});

app.Run();