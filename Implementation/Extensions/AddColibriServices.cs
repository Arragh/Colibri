using Implementation.Services.Http;
using Interfaces.Services.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Implementation.Extensions;

public static class AddColibriServicesExtension
{
    public static void AddColibriServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpTransportProvider, HttpTransportProvider>();
    }
}