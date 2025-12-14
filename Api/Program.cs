using Implementation;
using Interfaces.Services;
using Interfaces.Services.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfiguration();
builder.Services.AddImplementedServices();

var app = builder.Build();

app.Map("/get", async (HttpContext context, IHttpTransportProvider transportProvider) =>
{
    var transport = transportProvider.GetHttpTransport("TestGo");
    var request = new HttpRequestMessage(HttpMethod.Get, "http://192.168.1.102:6000/get");
    request.Headers.ExpectContinue = false;
    var response = await transport.SendAsync(request, context.RequestAborted);
    context.Response.StatusCode = (int)response.StatusCode;
    await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
});

app.Map("/post", async (HttpContext context, IHttpTransportProvider transportProvider) =>
{
    var transport = transportProvider.GetHttpTransport("TestGo");
    var request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.1.102:6000/post");
    request.Headers.ExpectContinue = false; // Пробуем ускорить передачу, но это не точно

    if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
    {
        request.Content = new StreamContent(context.Request.Body);

        // if (!string.IsNullOrEmpty(context.Request.ContentType))
        // {
        //     request.Content.Headers.TryAddWithoutValidation("Content-Type", context.Request.ContentType);
        // }
    }

    foreach (var header in context.Request.Headers)
    {
        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
        {
            request.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    var response = await transport.SendAsync(request, context.RequestAborted);
    context.Response.StatusCode = (int)response.StatusCode;

    foreach (var header in response.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    foreach (var header in response.Content.Headers)
    {
        context.Response.Headers[header.Key] = header.Value.ToArray();
    }

    context.Response.Headers.Remove("transfer-encoding"); // По идее ASP должен сам его выставить, но это не точно
    await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
});


app.Run();