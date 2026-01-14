using Colibri.Configuration;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.ClusterMatcher;
using Colibri.Runtime.Snapshots;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<SnapshotProvider>();
builder.Services.AddSingleton<ClusterMatcherMiddleware>();

builder.Services.AddSingleton<PipelineSrv>(sp => new([
    sp.GetRequiredService<ClusterMatcherMiddleware>()
]));

var app = builder.Build();

var snapshotProvider = app.Services.GetRequiredService<SnapshotProvider>();
var pipeline = app.Services.GetRequiredService<PipelineSrv>();

app.Run(async ctx =>
{
    var pipelineCtx = new PipelineContext
    {
        GlobalSnapshot = snapshotProvider.GlobalSnapshot,
        HttpContext = ctx,
        CancellationToken = ctx.RequestAborted
    };
    
    await pipeline.ExecuteAsync(pipelineCtx);
});

app.Run();