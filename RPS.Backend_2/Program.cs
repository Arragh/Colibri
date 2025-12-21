var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/post", (RequestModel  request) =>
{
    if (request.Login != "user123" || request.Password != "pass123")
    {
        return Results.Forbid();
    }

    return Results.Ok("Logged in");
});

app.Run();

record RequestModel(string Login, string Password);