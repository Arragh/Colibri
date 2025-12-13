var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int count = 1;

app.MapGet("/get", () =>
{
    var response = "Hello World!";
    
    Interlocked.Increment(ref count);
    Console.WriteLine(count);
    
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

struct RequestModel
{
    public string Login { get; set; }
    public string Password { get; set; }
}