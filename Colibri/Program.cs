using Colibri.Configuration;
using Colibri.Extensions;
using Colibri.Services.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<ClusterSetting>()
    .BindConfiguration("ClusterSetting");

builder.Services.AddSingleton<HttpTransportProvider>();

var app = builder.Build();

app.MapColibriEndpoints();

app.Run();