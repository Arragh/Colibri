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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<ICircuitBreaker, CircuitBreaker>();
builder.Services.AddSingleton<ILoadBalancer, LoadBalancer>();
builder.Services.AddSingleton<IRateLimiter, RateLimiter>();

builder.Services.AddSingleton<CircuitBreakerMiddleware>();
builder.Services.AddSingleton<LoadBalancerMiddleware>();
builder.Services.AddSingleton<RateLimiterMiddleware>();
builder.Services.AddSingleton<TerminalMiddleware>();

builder.Services.AddSingleton<UnstableTerminalMiddleware>();
builder.Services.AddSingleton<RetryMiddleware>();
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

app.Map("/{**catchAll}", static async (
    Pipeline pipeline,
    HttpContext http) =>
{
    Console.WriteLine("TROLOLO");
    
    var ctx = new PipelineContext
    {
        HttpContext = http,
        CancellationToken = http.RequestAborted,
        ClusterId = 1,
        EndpointId = 42
    };

    await pipeline.ExecuteAsync(ctx);
});

app.Run();