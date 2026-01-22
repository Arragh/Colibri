var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/get", (string name, int age) =>
{
    var response = new
    {
        Name = name + " - Cluster-1",
        Age = age
    };

    return Results.Ok(response);
});

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