using Colibri.Configuration;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.ClusterEngine;
using Colibri.Runtime.Snapshots;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<SnapshotProvider>();
builder.Services.AddSingleton<ClusterEngine>();
builder.Services.AddSingleton<ClusterEngineMiddleware>();

builder.Services.AddSingleton<PipelineSrv>(sp => new([
    sp.GetRequiredService<ClusterEngineMiddleware>()
]));

var app = builder.Build();

// var snapshotProvider = app.Services.GetRequiredService<ISnapshotProvider>();
var pipeline = app.Services.GetRequiredService<PipelineSrv>();

app.Run(async ctx =>
{
    var pipelineCtx = new PipelineContext
    {
        HttpContext = ctx,
        CancellationToken = ctx.RequestAborted
    };
    
    // var clusterSnapshot = snapshotProvider.ClusterSnapshot;
    // var cluster = clusterSnapshot.Clusters.First();
    // await cluster.Pipeline.ExecuteAsync(pipelineCtx);
    
    await pipeline.ExecuteAsync(pipelineCtx);
    
    Console.WriteLine();
});

app.Run();