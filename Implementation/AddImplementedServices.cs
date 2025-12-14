using Implementation.Services;
using Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation;

public static class AddServicesExtension
{
    public static void AddImplementedServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpService, HttpService>();
    }
}