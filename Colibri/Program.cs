using Colibri.Configuration;
using Colibri.Services.RoutingState;
using Colibri.Services.RoutingState.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddColibriSettings();

builder.Services.AddSingleton<IRoutingState, RoutingState>();

var app = builder.Build();

app.Map("/{**catchAll}", static async (
    HttpContext ctx,
    IRoutingState routingState) =>
{
    foreach (var url in routingState.Snapshot.Clusters[0].BaseUrls)
    {
        Console.WriteLine(url);
    }
});

app.Run();