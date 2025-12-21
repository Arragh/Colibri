var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/get", (string name, int age) =>
{
    var response = new
    {
        Name = name,
        Age = age
    };

    return Results.Ok(response);
});

app.Run();