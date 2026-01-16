var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/get/{name}/info/{age}", (string name, int age) =>
{
    var response = new
    {
        Name = name,
        Age = age
    };

    return Results.Ok(response);
});

app.MapGet("/get/{name}", (string name) =>
{
    var response = new
    {
        Name = name,
        Age = 7
    };

    return Results.Ok(response);
});

app.Run();