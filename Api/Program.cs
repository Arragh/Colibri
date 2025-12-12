using Api.Configuration;
using Api.Interfaces;
using Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<EndpointsSettings>()
    .BindConfiguration("Configuration");

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpService, HttpService>();

var app = builder.Build();

// app.Map("/{**catchAll}", async (HttpContext context, IHttpService httpService) =>
app.Map("/hello", async (HttpContext context, IHttpService httpService) =>
{
    var invoker = httpService.GetClient("TestGo");
    var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080/hello");
    var response = await invoker.SendAsync(request, context.RequestAborted);
    context.Response.StatusCode = (int)response.StatusCode;
    await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
});

app.Run();