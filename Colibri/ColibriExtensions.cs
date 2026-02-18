using System.Threading.Channels;
using Colibri.Configuration;
using Colibri.Runtime.Pipeline;
using Colibri.Runtime.Pipeline.RoutingEngine;
using Colibri.Runtime.Snapshots;
using Colibri.Runtime.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Colibri;

public static class ColibriExtensions
{
    public static IServiceCollection AddColibriServices(this IServiceCollection services)
    {
        services
            .AddOptions<ColibriSettings>()
            .BindConfiguration("Colibri");

        services.AddSingleton<SnapshotProvider>();
        services.AddSingleton<RoutingEngineMiddleware>();
        services.AddSingleton(Channel.CreateUnbounded<IAsyncDisposable>());
        services.AddSingleton<PipelineSrv>(sp => new([
            sp.GetRequiredService<RoutingEngineMiddleware>()
        ]));

        services.AddHostedService<GlobalAsyncDisposer>();
        
        return services;
    }

    public static IApplicationBuilder UseColibri(this IApplicationBuilder app)
    {
        var snapshotProvider = app.ApplicationServices.GetRequiredService<SnapshotProvider>();
        var pipeline = app.ApplicationServices.GetRequiredService<PipelineSrv>();

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
        
        return app;
    }
}