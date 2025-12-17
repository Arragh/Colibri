using Api.Extensions;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddColibriInfrastructure();

var app = builder.Build();

app.MapColibriEndpoints();

app.Run();