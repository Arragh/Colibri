var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

int count = 1;

app.MapGet("/get", (string name, int age) =>
{
    var response = new
    {
        Name = name,
        Age = age
    };

    Console.WriteLine(count);
    Interlocked.Increment(ref count);

    return Results.Ok(response);
});

app.Run();