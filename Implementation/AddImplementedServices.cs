using Implementation.Services.Http;
using Interfaces.Services.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation;

public static class AddServicesExtension
{
    public static void AddImplementedServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpTransportProvider, HttpTransportProvider>();
    }
}