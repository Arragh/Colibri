var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.Map("/{**catchAll}", static async (HttpContext ctx) =>
{
    
});

app.Run();